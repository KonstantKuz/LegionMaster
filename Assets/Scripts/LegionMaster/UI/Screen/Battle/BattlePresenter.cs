using System.Collections;
using JetBrains.Annotations;
using LegionMaster.Campaign.Session.Messages;
using LegionMaster.Core.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Session.Messages;
using LegionMaster.Location.Session.Messages;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Screen.CampaignDebriefing;
using LegionMaster.UI.Screen.BattleMode;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.DuelDebriefing;
using LegionMaster.UI.Screen.HyperCasualMode;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.Units.Component;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Battle
{
    public class BattlePresenter : BaseScreen
    {
        private const ScreenId BATTLE_PLAY_SCREEN = ScreenId.BattlePlay;
        public static readonly string URL = BattleScreen.BATTLE_SCREEN + "/" + BATTLE_PLAY_SCREEN;
        public override ScreenId ScreenId => BATTLE_PLAY_SCREEN;
        public override string Url => URL;

        [SerializeField]
        private float _afterBattleDelay;

        [Inject]
        private ScreenSwitcher _screenSwitcher;
        [Inject]
        private IMessenger _messenger;
        [Inject]
        private IBattleSessionService _sessionService;

        [PublicAPI]
        public void Init(GameMode gameMode)
        {
            _sessionService.StartBattle();
        }

        private void OnEnable()
        {
            _messenger.Subscribe<BattleEndMessage>(OnBattleFinished);
            _messenger.Subscribe<RoundEndMessage>(OnRoundFinished);
            _messenger.Subscribe<StageEndMessage>(OnStageFinished);
        }

        private void OnStageFinished(StageEndMessage message)
        {
            StartCoroutine(EndStage(message));
        }

        private void OnRoundFinished(RoundEndMessage message)
        {
            StartCoroutine(EndRound(message));
        }

        private IEnumerator EndRound(RoundEndMessage msg)
        {
            yield return new WaitForSeconds(_afterBattleDelay);
            _screenSwitcher.SwitchTo(DuelDebriefingPresenter.URL, false, msg.Score[UnitType.PLAYER], msg.Score[UnitType.AI], msg.Winner,
                                     msg.FinishedMatch);
        }

        private IEnumerator EndStage(StageEndMessage msg)
        {
            yield return new WaitForSeconds(_afterBattleDelay);
            _screenSwitcher.SwitchTo(CampaignDebriefingPresenter.URL, false, msg.Stage, msg.Winner, msg.FinishedMatch);
        }

        private void OnBattleFinished(BattleEndMessage message)
        {
            StartCoroutine(EndBattle());
        }

        private void OnDisable()
        {
            _messenger.Unsubscribe<BattleEndMessage>(OnBattleFinished);
            _messenger.Unsubscribe<RoundEndMessage>(OnRoundFinished); 
            _messenger.Unsubscribe<StageEndMessage>(OnStageFinished);
        }

        private IEnumerator EndBattle()
        {
            yield return new WaitForSeconds(_afterBattleDelay);
            var battleSession = _sessionService.FinishBattle();
            _screenSwitcher.SwitchTo(DebriefingScreenPresenter.URL, false, battleSession,
                                     AppConfig.IsHyperCasual ? HyperCasualModePresenter.URL : BattleModePresenter.URL);
        }
    }
}