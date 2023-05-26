using System.Collections;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Session.Messages;
using LegionMaster.Location.Session.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using LegionMaster.UI;
using LegionMaster.UI.Screen;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.DuelSquad;
using LegionMaster.UI.Screen.Main;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.Tutorial.Scenario
{
    public class LaunchTutorial : TutorialScenario
    {
        private const int PLACED_UNIT_COUNT = 3;
        
        private static readonly string[] FIRST_ROUND_UNITS = {"UnitBatter", "UnitDemoman", "UnitPyromaniac",};
        private static readonly string[] SECOND_ROUND_UNITS = {"UnitShieldBearer", "UnitShieldBearer", "UnitShieldBearer",};
        private static readonly string[] THIRD_ROUND_UNITS = FIRST_ROUND_UNITS;

        [SerializeField] private Transform _messagePosition;
        [SerializeField] private float _timeBeforeStartBattle = 1f;

        private DuelSquadPresenter _duelSquadPresenter;

        [Inject] private PlayerProgressService _playerProgress;
        [Inject] private IMessenger _messenger;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private SessionBuilder _sessionBuilder;
        [Inject] private DuelConfig _duelConfig;
        [Inject] private UIRoot _uiRoot;

        private DuelSquadPresenter DuelSquadPresenter => _duelSquadPresenter ??= _uiRoot.gameObject.GetComponentInChildren<DuelSquadPresenter>();

        public override bool IsStartAllowed(string screenUrl)
        {
            return screenUrl == MainScreenPresenter.URL && _playerProgress.GetWinBattleCount(GameMode.Duel) <= 0;
        }

        public override IEnumerator Run()
        {
            _sessionBuilder.CreateDuel();
            yield return RunRound1Step();
            yield return RunRound2Steps();
            yield return RunRound3Steps();
        }

        private IEnumerator RunRound1Step()
        {
            PrepareToRound1();
            ShowPopupWithoutNpcStep("LaunchTutorialMessageStepId1", _messagePosition);
            for (int i = 0; i < PLACED_UNIT_COUNT; i++) {
                yield return DuelSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();
            }
            UiTools.TutorialPopup.Hide();
            yield return new WaitForSeconds(_timeBeforeStartBattle);
            _screenSwitcher.SwitchTo(BattlePresenter.URL, false, GameMode.Duel);
            yield return new WaitForMessage<RoundEndMessage>(_messenger);
        }

        private void PrepareToRound1()
        {
            _screenSwitcher.SwitchTo(DuelSquadPresenter.URL, false, false);
            SetDisplayedUnits(FIRST_ROUND_UNITS);
        }

        private void SetDisplayedUnits(string[] unitIds)
        {
            DuelSquadPresenter.SetDisplayedUnits(unitIds);
            DuelSquadPresenter.EnsurePlayerHasEnoughTokens();
            DuelSquadPresenter.SetUpdateButtonEnabled(false); //it will be reenabled when screen will be reopened
        }

        private IEnumerator RunRound2Steps()
        {
            yield return new WaitForTutorialElementActivated("DuelSquadScreen");
            PrepareToRound2();
            ShowPopupWithoutNpcStep("LaunchTutorialMessageStepId2", _messagePosition);
            yield return DuelSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();

            yield return ShowPopupStep("LaunchTutorialMessageStepId3");

            ShowPopupWithoutNpcStep("LaunchTutorialMessageStepId4", _messagePosition);
         
            yield return DuelSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();

            yield return ShowPopupStep("LaunchTutorialMessageStepId5");
            yield return new WaitForSeconds(_timeBeforeStartBattle);
            _screenSwitcher.SwitchTo(BattlePresenter.URL, false, GameMode.Duel);
            yield return new WaitForMessage<RoundEndMessage>(_messenger);
        }

        private void PrepareToRound2()
        {
            DuelSquadPresenter.Dispose();
            _screenSwitcher.SwitchTo(DuelSquadPresenter.URL, false, false);
            SetDisplayedUnits(SECOND_ROUND_UNITS);
        }

        private IEnumerator RunRound3Steps()
        {
            yield return new WaitForTutorialElementActivated("DuelSquadScreen");
            PrepareToRound3();
            yield return ShowPopupStep("LaunchTutorialMessageStepId6");
            yield return ShowPopupStep("LaunchTutorialMessageStepId7");

            var timerHighlight = TutorialUiElementObserver.Get("TimerHighlight");
            PrepareTimerToHighlight(timerHighlight);
            UiTools.RectHighlighter.Set(timerHighlight);
            UiTools.TutorialHand.ShowOnElement(timerHighlight.PointerPosition, HandDirection.Right);
            yield return ShowPopupStep("LaunchTutorialMessageStepId8");
            ResetTimerFromHighlight();
            UiTools.RectHighlighter.Clear();
            UiTools.TutorialHand.Hide();

            DuelSquadPresenter.Dispose();
            _screenSwitcher.SwitchTo(DuelSquadPresenter.URL, false, true);

            ShowPopupWithoutNpcStep("LaunchTutorialMessageStepId9", _messagePosition);
            yield return new WaitForMessage<RoundStartMessage>(_messenger);
            UiTools.TutorialPopup.Hide();
            var waitForBattleEndMessage = new WaitForMessage<DuelBattleEndMessage>(_messenger);
            yield return waitForBattleEndMessage;
            OnDuelBattleFinished(waitForBattleEndMessage.Message);
            CompleteSteps();
        }

        private void PrepareToRound3()
        {
            DuelSquadPresenter.Dispose();
            _screenSwitcher.SwitchTo(DuelSquadPresenter.URL, false, false);
            SetDisplayedUnits(THIRD_ROUND_UNITS);
        }

        private void PrepareTimerToHighlight(TutorialUiElement timerHighlight)
        {
            DuelSquadPresenter.RoundInfoView.PrepareToHighlight(timerHighlight.transform, _duelConfig.SecondsBeforeStartRound - 1);
        }

        private void ResetTimerFromHighlight()
        {
            DuelSquadPresenter.RoundInfoView.ResetFromHighlight();
        }

        private void OnDuelBattleFinished(DuelBattleEndMessage evn)
        {
            if (!IsRunning) {
                return;
            }
            if (evn.IsPlayerWon) {
                FinishScenario();
            } else {
                TutorialService.ResetCurrentScenario();
            }
        }
    }
}