using System;
using JetBrains.Annotations;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Campaign;
using LegionMaster.UI.Screen.CampaignSquad;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Main;
using LegionMaster.Units.Component;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.CampaignDebriefing
{
    public class CampaignDebriefingPresenter : BaseScreen
    {
        private const ScreenId CAMPAIGN_DEBRIEFING_SCREEN = ScreenId.CampaignDebriefing;
        
        public static readonly string URL = CampaignScreen.CAMPAIGN_SCREEN + "/" + CAMPAIGN_DEBRIEFING_SCREEN;
        public override ScreenId ScreenId => CAMPAIGN_DEBRIEFING_SCREEN; 
        public override string Url => URL;

        [SerializeField] private GameObject _textContainer;
        [SerializeField] private float _textDisplayTime;
        [SerializeField] private TextMeshProLocalization _stageText;

        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private BattleSessionServiceWrapper _sessionService;
        [Inject] private SessionBuilder _sessionBuilder;
        [Inject] private LocationObjectFactory _locationObjectFactory;
        
        private CompositeDisposable _disposable;
        private bool _finishedMatch;

        [PublicAPI]
        public void Init(int stage, UnitType winner, bool finishedMatch)
        {
            _finishedMatch = finishedMatch;
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            if (winner == UnitType.PLAYER) {
                _textContainer.SetActive(true);
                _stageText.SetTextFormatted(_stageText.LocalizationId, stage);
                Observable.Timer(TimeSpan.FromSeconds(_textDisplayTime)).Subscribe(it => PlayTransition()).AddTo(_disposable);
            } else {
                Close();
            }
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        
        private void PlayTransition()
        {
            _textContainer.SetActive(false);
            _screenSwitcher.SwitchTo(UnitsTransitionPresenter.URL, false, (Action) Close, _finishedMatch);
        }
        
        private void Close()
        {
            _locationObjectFactory.DestroyAllObjects();

            if (!_finishedMatch)
            {
                StartNextStage();
            } else
            {
                FinishChapter();
            }
        }

        private void StartNextStage()
        {
            _sessionBuilder.CreateNextCampaignStage();
            _screenSwitcher.SwitchTo(CampaignSquadPresenter.URL, true);
        }

        private void FinishChapter()
        {
            var battleSessionResult = _sessionService.FinishBattle();
            _screenSwitcher.SwitchTo(DebriefingScreenPresenter.URL,
                true,
                battleSessionResult,
                MainScreenPresenter.URL);
        }
    }
}