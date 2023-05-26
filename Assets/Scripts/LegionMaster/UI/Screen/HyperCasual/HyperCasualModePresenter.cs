using System;
using System.Collections;
using JetBrains.Annotations;
using LegionMaster.Analytics.Data;
using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.HyperCasual.Config;
using LegionMaster.HyperCasual.Store;
using LegionMaster.HyperCasual.Store.Data;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.BattleMode.View;
using LegionMaster.UI.Screen.HyperCasualMode.Model;
using LegionMaster.UI.Screen.HyperCasualMode.Model.View;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.FreeCellProvider;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using LegionMaster.Units.Component;
using ModestTree;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.HyperCasualMode
{
    public class HyperCasualModePresenter : BaseScreen
    {
        private const ScreenId HYPERCASUAL_MODE_SCREEN = ScreenId.HyperCasual;
        public static readonly string URL = HYPERCASUAL_MODE_SCREEN.ToString();
        public override ScreenId ScreenId => HYPERCASUAL_MODE_SCREEN;
        public override string Url => URL;

        [SerializeField] private UnitCursor _unitCursor;
        [SerializeField] private ActionButton _toBattleButton;
        [SerializeField] private HyperCasualShopButtonsView _hyperCasualShopButtonsView;
        [SerializeField] private float _cameraSwitchTime = 0.5f;
        [SerializeField] private EnemyLevelView _enemyLevelView;
        [SerializeField] private HyperCasualUnitMerger _unitMerger;

        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private LocationArena _locationArena;
        [Inject] private CameraManager _cameraManager;
        [Inject] private EnemySquadService _enemySquad;
        [Inject] private SessionBuilder _sessionBuilder;
        [Inject] private IMessenger _messenger;   
        [Inject] private HyperCasualStoreService _storeService;
        [Inject] private Analytics.Analytics _analytics;
        [Inject] private HyperCasualUnitConfig _hyperCasualUnitConfig;
        private CompositeDisposable _disposable;
        private HyperCasualModeScreenModel _model;
        private IFreeCellProvider _freeCellProvider;
        private UnitPlacer _unitPlacer;

        public HyperCasualPlayerSquadSetup SquadSetup => (HyperCasualPlayerSquadSetup) _locationArena.CurrentPlayerSquadSetup;
        public UnitCursor UnitCursor => _unitCursor;
        
        [PublicAPI]
        public void Init()
        {
            Assert.IsNull(_disposable);
            _disposable = new CompositeDisposable();
            SwitchMode();
            CreateTools();
            InitModel();
            InitStartButton();
        }
        private void SwitchMode()
        {
            _locationArena.CurrentMode = GameMode.HyperCasual;
            _sessionBuilder.CreateHyperCasual();
            _locationArena.ShowSquadSetups();
            _cameraManager.SwitchTo(CameraManager.BATTLE_MODE, _cameraSwitchTime);
        }
        private void CreateTools()
        {
            _unitPlacer = new UnitPlacer(SquadSetup, _unitCursor);
            _freeCellProvider = new RandomFreeCellProvider(SquadSetup);
            _unitMerger.Init(SquadSetup, _unitCursor, _unitPlacer, _hyperCasualUnitConfig);
        }
        private void InitModel()
        {
            _model = new HyperCasualModeScreenModel(_enemySquad, 
                                                    _hyperCasualUnitConfig.MeleeUnitsConfig, _hyperCasualUnitConfig.RangedUnitsConfig, 
                                                    _storeService, SquadSetup.PlacedUnits, _locationArena.CurrentGrid.GetCells(UnitType.PLAYER).Count);

            _hyperCasualShopButtonsView.Init(_model.PriceButtons, BuyUnit);
            _enemyLevelView.Init(_model.EnemyLevel);
            
            SquadSetup.OnStartUnitDrag += OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag += OnFinishUnitDrag;
        }
        private void InitStartButton()
        {
            _toBattleButton.Init(StartBattle);
            _model.CanStartBattle
                .Subscribe(value => _toBattleButton.gameObject.SetActive(value)).AddTo(_disposable);
        }

        private void StartBattle()
        {
            _screenSwitcher.SwitchTo(BattlePresenter.URL, false, GameMode.HyperCasual);
        }

        private void BuyUnit(MergeableUnitType unitType)
        {
            using (_analytics.SetAcquisitionProperties(ScreenId.HyperCasual.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                if (!_storeService.TryBuy(unitType)) {
                    throw new Exception($"Can't buy mergeable unit, unitType:= {unitType}");
                }
                _model.UpdatePriceButton(unitType);
                PlacePurchasedUnit(_model.BuildDefaultUnit(unitType));
            }
        }
        
        private void PlacePurchasedUnit(PlacedUnitModel unit)
        {
            SquadSetup.AddToModelList(unit);
            _unitPlacer.PlaceInCellNewUnit(unit, _freeCellProvider.GetFreeCell());
        }

        private void OnStartUnitDrag(PlacedUnitModel placedUnit, GameObject obj)
        {
            if (_unitCursor.IsUnitAttached) {
                return;
            }
            SquadSetup.SetCellFullState(placedUnit, false);
            _unitCursor.Attach(obj);
        }

        private void OnFinishUnitDrag(PlacedUnitModel unit, GameObject obj)
        {
            if (_unitCursor.AttachedUnit != obj) {
                return;
            }
            if (_unitCursor.SelectedCell == null) {
                _unitPlacer.PlaceInCell(unit, SquadSetup.GetGridCellById(unit.CellId));
                return;
            }
            if (_unitCursor.SelectedCell.IsFull && TryMerge(unit)) {
                return;
            }

            _unitPlacer.PlaceInCell(unit, _unitCursor.SelectedCell);
            _messenger.Publish(new UnitPlaceChangedMessage());
        }

        private bool TryMerge(PlacedUnitModel unit)
        {
            var targetUnit = SquadSetup.GetPlacedUnitByCellId(_unitCursor.SelectedCell.Id);
            if (!_unitMerger.CanMerge(unit, targetUnit))
            {
                return false;
            }
            _unitMerger.Merge(unit, targetUnit);
            _messenger.Publish(new UnitPlaceChangedMessage());
            return true;
        }

        public override IEnumerator Hide()
        {
            Dispose();
            return base.Hide();
        }

        private void Dispose()
        {
            SquadSetup.OnStartUnitDrag -= OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag -= OnFinishUnitDrag;

            _model = null;
            _unitPlacer.ClearCursor();
            _locationArena.HideSquadSetups();

            _disposable?.Dispose();
            _disposable = null;
        }

        public void SetButtonsInteractable(bool value)
        {
            _toBattleButton.SetInteractable(value);
            _hyperCasualShopButtonsView.SetButtonsInteractable(value);
        }
    }
}