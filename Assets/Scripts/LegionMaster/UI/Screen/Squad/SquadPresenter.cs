using System;
using System.Collections;
using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.BattleMode.View;
using LegionMaster.UI.Screen.Squad.FreeCellProvider;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using LegionMaster.UI.Screen.Squad.View;
using LegionMaster.Units.Config;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace LegionMaster.UI.Screen.Squad
{
    public class SquadPresenter : BaseScreen
    {
        public const ScreenId SQUAD_SCREEN = ScreenId.Squad;

        public static readonly string URL = SQUAD_SCREEN.ToString();
        public override ScreenId ScreenId => SQUAD_SCREEN;
        public override string Url => URL;

        [SerializeField] private UnitCursor _unitCursor;
        [SerializeField] private UnitButtonsView _unitButtonsView;
        [SerializeField] private ActionButton _startButton;
        [SerializeField] private HideDownView _hideDownView;
        [SerializeField] private float _cameraSwitchTime = 0.5f;
        [SerializeField] private TextView _placedUnitCountText;
        [SerializeField] private EnemyLevelView _enemyLevelView;
        
        [Inject] private CameraManager _cameraManager;
        [Inject] private LocationArena _locationArena;
        [Inject] private InventoryService _inventoryService;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private IMessenger _messenger;     
        [Inject] private PlayerSquadService _playerSquadService;
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private EnemySquadService _enemySquad;
        
        private IFreeCellProvider _freeCellProvider;
        private SquadScreenModel _model;
        private UnitPlacer _unitPlacer;
        
        private CompositeDisposable _disposable;
        private Action<Unit> _startUnitDragEvent;
        private Action<Unit> _stopUnitDragEvent;

        public BattlePlayerSquadSetup SquadSetup => (BattlePlayerSquadSetup) _locationArena.CurrentPlayerSquadSetup;
        
        public IFreeCellProvider DefaultFreeCellProvider => new RandomFreeCellProvider(SquadSetup);
        public IObservable<Unit> StartUnitDragAsObservable => Observable.FromEvent<Unit>(handler => _startUnitDragEvent += handler, 
                                                                                         handler => _startUnitDragEvent -= handler);
        public IObservable<Unit> StopUnitDragAsObservable => Observable.FromEvent<Unit>(handler => _stopUnitDragEvent += handler, 
                                                                                        handler => _stopUnitDragEvent -= handler);
        protected override void Awake()
        {
            base.Awake();
            _freeCellProvider = DefaultFreeCellProvider;
        }
        private void OnEnable()
        {
            _cameraManager.SwitchTo(CameraManager.BATTLE_SQUAD_MODE, _cameraSwitchTime);
            PlayShowAnimation();
            InitModel();
            InitStartButton();
        }
        private void PlayShowAnimation()
        {
            _hideDownView.ShowSmoothly(_cameraSwitchTime);
        }
        private void InitModel()
        {
            Assert.IsNull(_disposable);
            _disposable = new CompositeDisposable();
            _unitPlacer = new UnitPlacer(SquadSetup, _unitCursor);
            
            _model = new SquadScreenModel(SquadSetup.PlacedUnits, _inventoryService, _playerSquadService, _enemySquad, OnClickUnitButton, _unitCollectionConfig);
            _unitButtonsView.Init(_model.UnitButtons);
            _placedUnitCountText.Init(_model.PlacedUnitCountText);
            _enemyLevelView.Init(_model.EnemyLevel);
                    
            SquadSetup.OnStartUnitDrag += OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag += OnFinishUnitDrag;
        }
        private void InitStartButton()
        {
            _startButton.Init(OnStartBattle);
            _model.CanStartBattle.Subscribe(canStart => _startButton.Button.interactable = canStart).AddTo(_disposable);
        }

        private void OnStartBattle()
        {
            _screenSwitcher.SwitchTo(BattlePresenter.URL, false, GameMode.Battle);
        }
        private void OnStartUnitDrag(PlacedUnitModel placedUnit, GameObject obj)
        {
            if (_unitCursor.IsUnitAttached) {
                return;
            }
            SquadSetup.SetCellFullState(placedUnit, false);
            _unitCursor.Attach(obj);
            _startUnitDragEvent?.Invoke(Unit.Default);
        }
        private void OnFinishUnitDrag(PlacedUnitModel unit, GameObject obj)
        {
            if (_unitCursor.AttachedUnit != obj) {
                return;
            }
            StopPlacing(unit);
            _model.UpdateModel();
            _stopUnitDragEvent?.Invoke(Unit.Default);
        }
        private void StopPlacing(PlacedUnitModel unit)
        {
            if (_unitCursor.RemovingArea) {
                _unitPlacer.PlaceInCell(unit, null);
                return;
            }
            if (_unitCursor.SelectedCell == null) {
                _unitPlacer.PlaceInCell(unit, SquadSetup.GetGridCellById(unit.CellId));
                return;
            }
            _unitPlacer.PlaceInCell(unit, _unitCursor.SelectedCell);
            _messenger.Publish(new UnitPlaceChangedMessage());
        }
        
        private void OnClickUnitButton(PlacedUnitModel unit)
        {
            if (unit.IsPlaced) {
                SquadSetup.RemoveUnit(unit);   
                _unitPlacer.SetCellId(unit, CellId.InvalidCellId);
                _model.UpdateModel();
                return;
            }
            _unitCursor.Attach(SquadSetup.LoadUnit(unit));
            _unitPlacer.PlaceInCell(unit, _freeCellProvider.GetFreeCell());
            _model.UpdateModel();
        }

        public override IEnumerator Hide()
        {
            DisposeModel();
            yield return _hideDownView.HideSmoothly(_cameraSwitchTime);
            yield return base.Hide();
        }

        private void DisposeModel()
        {
            SquadSetup.OnStartUnitDrag -= OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag -= OnFinishUnitDrag;
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }

        public void SetFreeCellProvider(IFreeCellProvider provider)
        {
            Assert.IsNotNull(provider);
            _freeCellProvider = provider;
        }

    }
}