using System;
using System.Collections;
using JetBrains.Annotations;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Duel;
using LegionMaster.UI.Screen.DuelSquad;
using LegionMaster.UI.Screen.Main;
using LegionMaster.Units.Component;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.DuelDebriefing
{
    public class DuelDebriefingPresenter : BaseScreen
    {
        private const ScreenId DUEL_DEBRIEFING_SCREEN = ScreenId.DuelDebriefing;
        public static readonly string URL = DuelScreen.DUEL_SCREEN + "/" + DUEL_DEBRIEFING_SCREEN;
        public override ScreenId ScreenId => DUEL_DEBRIEFING_SCREEN; 
        public override string Url => URL;

        [SerializeField] private ScoreView _enemyScoreView;
        [SerializeField] private ScoreView _playerScoreView;
        [SerializeField] private float _displayTimeSeconds;

        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private BattleSessionServiceWrapper _sessionService;
        [Inject] private SessionBuilder _sessionBuilder;
        [Inject] private LocationObjectFactory _locationObjectFactory;
        
        private CompositeDisposable _disposable;
        private bool _finishedMatch;        
        private UnitType _winner;

        [PublicAPI]
        public void Init(int playerScore, int enemyScore, UnitType winner, bool finishedMatch)
        {
            _winner = winner;
            _finishedMatch = finishedMatch;
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            _enemyScoreView.Init(enemyScore, winner == UnitType.AI);
            _playerScoreView.Init(playerScore, winner == UnitType.PLAYER);
            Observable.Timer(TimeSpan.FromSeconds(_displayTimeSeconds)).Subscribe(it => Close()).AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        
        public override IEnumerator Hide()
        {
            _locationObjectFactory.DestroyAllObjects();
            yield return base.Hide();
        } 
        private void Close()
        {
            if (!_finishedMatch) {
                _sessionBuilder.CreateNextDuelRound(_winner);
                _screenSwitcher.SwitchTo(DuelSquadPresenter.URL, true, true);
            } else {
                var battleSessionResult = _sessionService.FinishBattle();
                _screenSwitcher.SwitchTo(DebriefingScreenPresenter.URL, 
                                         true, 
                                         battleSessionResult, 
                                         MainScreenPresenter.URL);
            }
        }
    }
}