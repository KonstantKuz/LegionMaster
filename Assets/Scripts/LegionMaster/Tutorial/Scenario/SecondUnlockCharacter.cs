using System.Collections;
using LegionMaster.BattlePass.Model;
using LegionMaster.Tutorial.Model;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using LegionMaster.UI;
using LegionMaster.UI.Screen.BattlePass.View;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Footer;
using LegionMaster.UI.Screen.Footer.Model;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.Tutorial.Scenario
{
    public class SecondUnlockCharacter : TutorialScenario
    {
        private ScrollRect _battlePassScroll;

        [Inject]
        private ABTest.ABTest _abTest;
        [Inject]
        private UIRoot _uiRoot;

        private ScrollRect BattlePassScrollRect =>
                _battlePassScroll ??= _uiRoot.gameObject.GetComponentInChildren<BattlePassLevelListView>().ScrollRect;

        public override bool IsStartAllowed(string screenUrl)
        {
            return screenUrl == DebriefingScreenPresenter.URL && TutorialService.IsScenarioCompleted(TutorialScenarioId.LaunchCampaign)
                   && !_abTest.NoUnlockCharacter;
        }

        public override IEnumerator Run()
        {
            yield return new WaitForTutorialElementActivated("BattlePassScreen");
            TutorialService.CompleteScenario(ScenarioId);

            yield return ShowPopupStep("Unlock2TutorialMessageStepId1");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId2");

            BattlePassScrollRect.vertical = false;

            yield return ClickUiElementStep(BattlePassRewardView.GetTutorialId(new BattlePassRewardId(1, BattlePassRewardType.Basic)));
            yield return ClickUiElementStep(BattlePassRewardView.GetTutorialId(new BattlePassRewardId(2, BattlePassRewardType.Basic)));

            BattlePassScrollRect.vertical = true;
            yield return ClickUiElementStep("BattlePassNextButton", HandDirection.Down);

            yield return ShowPopupStep("Unlock2TutorialMessageStepId6");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId7");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId8");

            yield return ClickUiElementStep("ProgressUnitUpgradeButton");

            yield return ShowPopupStep("Unlock2TutorialMessageStepId10");

            yield return ClickUiElementStep("CharacterCraftButton");

            yield return ShowPopupStep("Unlock2TutorialMessageStepId12");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId13");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId14");

            yield return ClickUiElementStep("CharacterUpgradeButton");

            yield return ShowPopupStep("Unlock2TutorialMessageStepId16");
            yield return ShowPopupStep("Unlock2TutorialMessageStepId17");

            yield return ClickUiElementStep(FooterButtonView.GetTutorialId(FooterButtonType.Campaign), HandDirection.Down);
            yield return ClickUiElementStep("MainScreenStartButton");
            CompleteSteps();
            FinishScenario();
        }
    }
}