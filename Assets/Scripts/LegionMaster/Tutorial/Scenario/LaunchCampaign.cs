using System.Collections;
using LegionMaster.Campaign.Session.Messages;
using LegionMaster.Core.Mode;
using LegionMaster.Location.Session.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using LegionMaster.UI;
using LegionMaster.UI.Screen;
using LegionMaster.UI.Screen.CampaignSquad;
using LegionMaster.UI.Screen.Main;
using LegionMaster.Units.Component;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.Tutorial.Scenario
{
    public class LaunchCampaign : TutorialScenario
    {
        private const int PLACED_UNIT_COUNT = 3;

        private static readonly string[] FIRST_STAGE_UNITS = {"UnitBatter", "UnitDemoman", "UnitPyromaniac"};
        private static readonly string[] SECOND_STAGE_UNITS = {"UnitShieldBearer", "UnitShieldBearer"};
        private static readonly string[] THIRD_STAGE_UNITS = FIRST_STAGE_UNITS;

        [SerializeField]
        private Transform _messagePosition;

        private CampaignSquadPresenter _campaignSquadPresenter;

        [Inject]
        private PlayerProgressService _playerProgress;
        [Inject]
        private IMessenger _messenger;
        [Inject]
        private ScreenSwitcher _screenSwitcher;
        [Inject]
        private SessionBuilder _sessionBuilder;
        [Inject]
        private UIRoot _uiRoot;

        private CampaignSquadPresenter CampaignSquadPresenter =>
                _campaignSquadPresenter ??= _uiRoot.gameObject.GetComponentInChildren<CampaignSquadPresenter>();

        public override bool IsStartAllowed(string screenUrl)
        {
            return screenUrl == MainScreenPresenter.URL && _playerProgress.GetWinBattleCount(GameMode.Campaign) <= 0;
        }

        public override IEnumerator Run()
        {
            _sessionBuilder.CreateCampaign();
            yield return RunStage1();
            yield return RunStage2();
            yield return RunStage3();
        }

        private IEnumerator RunStage1()
        {
            PrepareToStage1();
            ShowPopupWithoutNpcStep("LaunchCampaignMessageStepId1", _messagePosition);
            for (int i = 0; i < PLACED_UNIT_COUNT; i++) {
                yield return CampaignSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();
            }
            UiTools.TutorialPopup.Hide();
            yield return ClickCampaignFightButton();
            yield return new WaitForMessage<StageEndMessage>(_messenger);
        }

        private IEnumerator ClickCampaignFightButton()
        {
            yield return ClickUiElementStep("CampaignFightButton", HandDirection.Up, false);
        }

        private void PrepareToStage1()
        {
            _screenSwitcher.SwitchTo(CampaignSquadPresenter.URL);
            SetDisplayedUnits(FIRST_STAGE_UNITS);
        }

        private void SetDisplayedUnits(string[] unitIds)
        {
            CampaignSquadPresenter.SetDisplayedUnits(unitIds);
            CampaignSquadPresenter.SetAvailableUnitCount(unitIds.Length);
            CampaignSquadPresenter.UseStore = false;
        }

        private IEnumerator RunStage2()
        {
            yield return new WaitForTutorialElementActivated("CampaignSquadScreen");
            SetDisplayedUnits(SECOND_STAGE_UNITS);
            
            ShowPopupWithoutNpcStep("LaunchCampaignMessageStepId3", _messagePosition);
            yield return CampaignSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();

            yield return ShowPopupStep("LaunchCampaignMessageStepId4");

            ShowPopupWithoutNpcStep("LaunchCampaignMessageStepId5", _messagePosition);
            yield return CampaignSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();

            yield return ShowPopupStep("LaunchCampaignMessageStepId6");
            yield return ClickCampaignFightButton();
            yield return new WaitForMessage<StageEndMessage>(_messenger);
        }

        private IEnumerator RunStage3()
        {
            yield return new WaitForTutorialElementActivated("CampaignSquadScreen");
            SetDisplayedUnits(THIRD_STAGE_UNITS);
            yield return ShowPopupStep("LaunchCampaignMessageStepId8");
            yield return ShowPopupStep("LaunchCampaignMessageStepId9");
            yield return ShowPopupStep("LaunchCampaignMessageStepId10");

            ShowPopupWithoutNpcStep("LaunchCampaignMessageStepId11", _messagePosition);
            for (int i = 0; i < PLACED_UNIT_COUNT; i++) {
                yield return CampaignSquadPresenter.StopUnitPlacingAsObservable.First().ToYieldInstruction();
            }
            UiTools.TutorialPopup.Hide();
            yield return ClickCampaignFightButton();
            var waitForStageEndMessage = new WaitForMessage<StageEndMessage>(_messenger);
            yield return waitForStageEndMessage;
            OnCampaignBattleFinished(waitForStageEndMessage.Message);
            CompleteSteps();
        }

        private void OnCampaignBattleFinished(StageEndMessage evn)
        {
            if (!IsRunning) {
                return;
            }
            if (evn.Winner == UnitType.PLAYER) {
                FinishScenario();
            } else {
                TutorialService.ResetCurrentScenario();
            }
        }
    }
}