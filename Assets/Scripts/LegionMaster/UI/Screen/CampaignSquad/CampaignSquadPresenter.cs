using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Campaign.Config;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Campaign.Store;
using LegionMaster.Core.Mode;
using LegionMaster.Extension;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.BattleCountdown;
using LegionMaster.UI.Screen.Campaign;
using LegionMaster.UI.Screen.CampaignSquad.DisplayCase;
using LegionMaster.UI.Screen.CampaignSquad.Model;
using LegionMaster.UI.Screen.CampaignSquad.View;
using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.Model;
using LegionMaster.UI.Screen.DuelSquad.SquadSetup;
using LegionMaster.UI.Screen.DuelSquad.View;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using LegionMaster.UI.Screen.Squad.View;
using LegionMaster.Units.Config;
using ModestTree;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;


namespace LegionMaster.UI.Screen.CampaignSquad
{
    public class CampaignSquadPresenter : BaseScreen
    {
        private const ScreenId CAMPAIGN_SQUAD_SCREEN = ScreenId.CampaignSquad;
        public static readonly string URL = CampaignScreen.CAMPAIGN_SCREEN + "/" + CAMPAIGN_SQUAD_SCREEN;
        public override ScreenId ScreenId => CAMPAIGN_SQUAD_SCREEN; 
        public override string Url => URL;

        [SerializeField] private UnitCursor _unitCursor;
        [SerializeField] private DisplayCaseUnitView _uiDisplayCaseView;
        [SerializeField] private float _cameraSwitchTime = 0.5f;
        [SerializeField] private UnitMerger _unitMerger;
        [SerializeField] private HideDownView _fightButtonPanel;
        [SerializeField] private ActionButton _startButton;   
        [SerializeField] private UnitPlacingAdviceView _unitPlacingAdvice;
        
        [Inject] private CameraManager _cameraManager;     
        [Inject] private LocationArena _locationArena;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private IMessenger _messenger;  
        [Inject] private CampaignUnitStoreService _campaignUnitStoreService;
        [Inject] private UnitCollectionConfig _unitCollectionConfig; 
        [Inject] private CampaignConfig _campaignConfig;
        [Inject] private BattleSessionServiceWrapper _sessionService;

        private CampaignSquadScreenModel _model;
        private UnitPlacer _unitPlacer;
        private CompositeDisposable _disposable;
        
        private Action<Unit> _stopUnitPlacingEvent;

        private MergeablePlayerSquadSetup SquadSetup => (MergeablePlayerSquadSetup) _locationArena.CurrentPlayerSquadSetup;  
        private CampaignDisplayCase SceneDisplayCaseView => _locationArena.CampaignDisplayCase;
        private UnitDisplayCase UnitDisplayCase => SceneDisplayCaseView.UnitDisplayCase;
        private CampaignSessionService CampaignSessionService => _sessionService.GetImpl<CampaignSessionService>();
        public IObservable<Unit> StopUnitPlacingAsObservable => Observable.FromEvent<Unit>(handler => _stopUnitPlacingEvent += handler, 
                                                                                           handler => _stopUnitPlacingEvent -= handler);
        public bool UseStore { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();
            _startButton.Init(ShowPreBattleScreen);
        }

        [PublicAPI]
        public void Init()
        {
            UseStore = true;
            Assert.IsNull(_disposable);
            _disposable = new CompositeDisposable();

            _locationArena.CurrentMode = GameMode.Campaign;
            _locationArena.ShowSquadSetups();
            
            PlayShowAnimation();
            InitModel();
        }
        private void InitModel()
        {

            _unitPlacer = new UnitPlacer(SquadSetup, _unitCursor);
            _unitMerger.Init(SquadSetup, _unitCursor);

            _model = new CampaignSquadScreenModel(SquadSetup.PlacedUnits, _campaignUnitStoreService, _unitCollectionConfig, 
                                                  CampaignSessionService, _campaignConfig, 
                                                  StartPlacing, StopPlacing, HighlightRespawnedUnit); 
            SceneDisplayCaseView.Init(_model.DisplayedUnits); 
            _uiDisplayCaseView.Init(_model.DisplayedUnits, SceneDisplayCaseView.DisplayCaseItemPositions);
            _unitPlacingAdvice.Init(_model.UnitPlacingAdvice);
            _model.FightButtonEnabled.Where(enabled => enabled).Subscribe(it => ShowFightButton()).AddTo(_disposable);
            SquadSetup.OnStartUnitDrag += OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag += OnFinishUnitDrag;
        }
        public void SetDisplayedUnits(IEnumerable<string> unitIds)
        {
            CheckModelInitialized();
            _campaignUnitStoreService.ReturnUnits(_model.DisplayedUnits.Value.GetUnplacedOnGridUnitIds());
            _model.SetDisplayedUnits(unitIds);
        }

        public void SetAvailableUnitCount(int amount)
        {
            CheckModelInitialized();
            _model.SetAvailableUnitCount(amount);
        }

        private void ShowFightButton()
        {
            _cameraManager.SwitchTo(CameraManager.CAMPAIGN_PLACING_SQUAD, _cameraSwitchTime);
            _fightButtonPanel.ShowSmoothly(_cameraSwitchTime);
            DisposeDisplayCaseView();
        }
        private void ShowPreBattleScreen() => _screenSwitcher.SwitchTo(BattleCountdownPresenter.URL, false, GameMode.Campaign);

        private void PlayShowAnimation() => _cameraManager.SwitchToImmediately(CameraManager.CAMPAIGN_SQUAD_MODE);
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
            _model.UpdateDisplayCaseUnit(id, DisplayedUnitState.PlacedOnGrid);
            if (_unitMerger.CanMerge(id.UnitId, id.Star)) {
                _unitMerger.Merge(id.UnitId, id.Star);
            } else {
                PlaceNewUnit(id);
            }
            if (UseStore) {
                _model.AddMissingUnitFromStore(id);    
            } else {
                _model.UpdateMergeStatuses();
            }
            _messenger.Publish(new UnitPlaceChangedMessage());
            _stopUnitPlacingEvent?.Invoke(Unit.Default);
            _model.IncreaseReinforcedUnitCount();
        }

        private bool СanNotPlaceOrMerge(DisplayCaseUnitId id) =>
                _unitCursor.SelectedCell == null || (_unitCursor.SelectedCell.IsFull && !_unitMerger.CanMerge(id.UnitId, id.Star));
        
        private void PlaceNewUnit(DisplayCaseUnitId id)
        {
            var newPlacedUnit = _model.CreatePlacedUnit(id);
            SquadSetup.AddToModelList(newPlacedUnit);
            _unitPlacer.PlaceInCellNewUnit(newPlacedUnit, _unitCursor.SelectedCell);
        }
        
        private void ReturnUnitToDisplayCase(DisplayCaseUnitId id)
        {
            _model.UpdateDisplayCaseUnit(id, DisplayedUnitState.NotTaken);
            var unitObject = _unitCursor.AttachedUnit;
            _unitCursor.Detach();
            UnitDisplayCase.PlaceUnit(unitObject, _model.GetDisplayCaseUnit(id).Id.PlaceId);
        }
        
        private void HighlightRespawnedUnit(DisplayCaseUnitId missingId)
        {
            UnitDisplayCase.PlayRespawnEffect(missingId);
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
            DisposeModel();
            _fightButtonPanel.HideSmoothly(_cameraSwitchTime);
            yield return base.Hide();
        }
        private void DisposeDisplayCaseView()
        { 
            ReturnUnitsToStore();
            _unitPlacer.ClearCursor();
            SceneDisplayCaseView.Hide();
            _uiDisplayCaseView.gameObject.SetActive(false);
        }
        private void ReturnUnitsToStore()
        {
            if (UseStore) {
                _campaignUnitStoreService.ReturnUnits(_model.DisplayedUnits.Value.GetUnplacedOnGridUnitIds());
            }
        }
        private void CheckModelInitialized()
        {
            if (_model == null) throw new Exception("You should init presenter first");
        }
        private void DisposeModel()
        {
            SquadSetup.OnStartUnitDrag -= OnStartUnitDrag;
            SquadSetup.OnFinishUnitDrag -= OnFinishUnitDrag;
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}