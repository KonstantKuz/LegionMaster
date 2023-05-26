using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Session.Service;
using LegionMaster.Duel.Store;
using LegionMaster.Extension;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.Location.Session.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.CampaignSquad;
using LegionMaster.UI.Screen.Duel;
using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.Model;
using LegionMaster.UI.Screen.DuelSquad.SquadSetup;
using LegionMaster.UI.Screen.DuelSquad.View;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using LegionMaster.Units.Config;
using LegionMaster.Util;
using ModestTree;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.DuelSquad
{
    public class DuelSquadPresenter : BaseScreen
    {
        private const ScreenId DUEL_SQUAD_SCREEN = ScreenId.DuelSquad;
        public static readonly string URL = DuelScreen.DUEL_SCREEN + "/" + DUEL_SQUAD_SCREEN;
        public override ScreenId ScreenId => DUEL_SQUAD_SCREEN; 
        public override string Url => URL;

        [SerializeField] private UnitCursor _unitCursor;
        [SerializeField] private DuelDisplayCaseView _uiView;
        [SerializeField] private float _cameraSwitchTime = 0.5f;
        
        [SerializeField] private RoundInfoView _roundInfoView;
        [SerializeField] private UnitMerger _unitMerger;
                
        [Inject] private CameraManager _cameraManager;     
        [Inject] private LocationArena _locationArena;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private IMessenger _messenger;  
        [Inject] private DuelUnitStoreService _duelUnitStoreService; 
        [Inject] private WalletService _walletService;     
        [Inject] private DuelConfig _duelConfig;    
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private Analytics.Analytics _analytics;      
        [Inject] private BattleSessionServiceWrapper _battleSessionService;

        
        private DuelSquadScreenModel _model;
        private UnitPlacer _unitPlacer;
        private CompositeDisposable _disposable;
        
        private Action<Unit> _stopUnitPlacingEvent;
        
        private MergeablePlayerSquadSetup SquadSetup => (MergeablePlayerSquadSetup) _locationArena.CurrentPlayerSquadSetup;   
        private DuelDisplayCase SceneView => _locationArena.DuelDisplayCase;
        private UnitDisplayCase UnitDisplayCase => SceneView.UnitDisplayCase;
        private DuelSessionService DuelSessionService => _battleSessionService.GetImpl<DuelSessionService>();

        public RoundInfoView RoundInfoView => _roundInfoView;
        public IObservable<Unit> StopUnitPlacingAsObservable => Observable.FromEvent<Unit>(handler => _stopUnitPlacingEvent += handler, 
                                                                                           handler => _stopUnitPlacingEvent -= handler);
      
        [PublicAPI]
        public void Init(bool timerEnabled)
        {
            _locationArena.CurrentMode = GameMode.Duel;
            _locationArena.ShowSquadSetups(false);
            PlayShowAnimation();
            Assert.IsNull(_disposable);
            _disposable = new CompositeDisposable();
            InitModel(timerEnabled);
        }
        private void InitModel(bool timerEnabled)
        {
            _unitPlacer = new UnitPlacer(SquadSetup, _unitCursor);
            _unitMerger.Init(SquadSetup, _unitCursor);
            
            _model = new DuelSquadScreenModel(SquadSetup.PlacedUnits, _duelUnitStoreService, _walletService, 
                                              _duelConfig, _unitCollectionConfig, DuelSessionService.DuelBattleSession, timerEnabled, StartPlacing, StopPlacing, BuyShopUpdate);
            SceneView.Init(_model.DisplayedUnits, _model.UpdateButton);
            _uiView.Init(_model.DisplayedUnits, _model.TokenAmount, _model.UpdateButton.Price, SceneView.DisplayCaseItemPositions);
            _roundInfoView.Init(_model.RoundInfo);
            _model.TimerEnabled.Where(enabled => enabled).Subscribe(it => InitTimer()).AddTo(_disposable);
            SquadSetup.OnStartUnitDrag += OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag += OnFinishUnitDrag;
        }

        private void InitTimer()
        {
            Timer.Create(DateTime.Now.AddSeconds(_duelConfig.SecondsBeforeStartRound), StartBattle).AddTo(_disposable);
        }

        private void StartBattle() => _screenSwitcher.SwitchTo(BattlePresenter.URL, true, GameMode.Duel);

        private void PlayShowAnimation()
        {
            _cameraManager.SwitchToImmediately(CameraManager.DUEL_SQUAD_MODE);
        }

        private void BuyUnit(DisplayCaseUnitId id)
        {
            if (!_duelUnitStoreService.TryBuyUnit(id.UnitId)) {
                throw new Exception($"Can't buy duel unit, unitId= {id.UnitId}");
            }
            _model.UpdateDisplayCaseUnit(id, DisplayedUnitState.PlacedOnGrid);
        }

        private void BuyShopUpdate()
        {
            if (!_duelUnitStoreService.TryBuyShopUpdate()) {
                throw new Exception("Try buy shop update error");
            }
            ReturnUnitsToStore();
            _model.UpdateDisplayedUnits();
            _analytics.DuelRerollPressed();
        }

        private void ReturnUnitsToStore()
        {
            _duelUnitStoreService.ReturnUnits(_model.DisplayedUnits.Value.GetUnplacedOnGridUnitIds());
        }

        private void StartPlacing(DisplayCaseUnitId id, GameObject unit)
        {
            if (_unitCursor.IsUnitAttached) {
                return;
            }
            _model.UpdateDisplayCaseUnit(id, DisplayedUnitState.Taken);
            unit.transform.ResetLocalTransform();
            _unitCursor.Attach(unit);
        }

        private void StopPlacing(DisplayCaseUnitId id, GameObject unit)
        {
            if (_unitCursor.AttachedUnit != unit) {
                return;
            }
            if (СanNotPlaceOrMerge(id)) {
                ReturnUnitToDisplayCase(id);
                return;
            }
            BuyUnit(id);
            if (_unitMerger.CanMerge(id.UnitId, id.Star)) {
                _unitMerger.Merge(id.UnitId, id.Star);
            } else {
                PlaceNewUnit(id);
            }
            _model.UpdateMergeStatuses();
            _messenger.Publish(new UnitPlaceChangedMessage());
            _stopUnitPlacingEvent?.Invoke(Unit.Default);
        }

        private bool СanNotPlaceOrMerge(DisplayCaseUnitId id) =>
                _unitCursor.SelectedCell == null || (_unitCursor.SelectedCell.IsFull && !_unitMerger.CanMerge(id.UnitId, id.Star));

  
        private void PlaceNewUnit(DisplayCaseUnitId id)
        {
            var newPlacedUnit = _model.CreatePlacedUnit(id);
            SquadSetup.AddToModelList(newPlacedUnit);
            _unitPlacer.PlaceInCellNewUnit(newPlacedUnit, _unitCursor.SelectedCell);
            _analytics.DuelUnitBought(id.UnitId);
        }
        
        private void ReturnUnitToDisplayCase(DisplayCaseUnitId id)
        {
            _model.UpdateDisplayCaseUnit(id, DisplayedUnitState.NotTaken);
            var unitObject = _unitCursor.AttachedUnit;
            _unitCursor.Detach();
            UnitDisplayCase.PlaceUnit(unitObject, _model.GetDisplayCaseUnit(id).Id.PlaceId);
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
            _unitPlacer.PlaceInCell(unit, _unitCursor.SelectedCell);
            _messenger.Publish(new UnitPlaceChangedMessage());
        }

        public override IEnumerator Hide()
        {
            DisposeView();
            DisposeModel();
            _locationArena.HideSquadSetups();
            SceneView.Hide();
            _cameraManager.SwitchTo(CameraManager.BATTLE_PLAY_MODE, _cameraSwitchTime);
            yield return base.Hide();
        }
        private void DisposeView()
        { 
            ReturnUnitsToStore();
            _unitPlacer.ClearCursor();
        }
        private void DisposeModel()
        {
            SquadSetup.OnStartUnitDrag -= OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag -= OnFinishUnitDrag;
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }

        public void SetUpdateButtonEnabled(bool buttonEnabled)
        {
            CheckModelInitialized();
            _model.UpdateButton.Enabled.SetValueAndForceNotify(buttonEnabled);
        }

        private void CheckModelInitialized()
        {
            if (_model == null) throw new Exception("You should init presenter first");
        }

        public void EnsurePlayerHasEnoughTokens()
        {
            CheckModelInitialized();
            _model.EnsurePlayerHasEnoughTokens();
        }

        public void SetDisplayedUnits(IEnumerable<string> unitIds)
        {
            _model.SetDisplayedUnits(unitIds);
        }

        public void Dispose()
        {
            DisposeView();
            DisposeModel();
        }
    }
}