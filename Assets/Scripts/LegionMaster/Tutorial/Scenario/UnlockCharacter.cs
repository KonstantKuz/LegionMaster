using System.Collections;
using LegionMaster.BattlePass.Model;
using LegionMaster.Tutorial.Model;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using LegionMaster.UI;
using LegionMaster.UI.Screen.BattlePass.View;
using LegionMaster.UI.Screen.Footer;
using LegionMaster.UI.Screen.Footer.Model;
using LegionMaster.UI.Screen.Main;
using LegionMaster.UI.Screen.Squad.View;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.Tutorial.Scenario
{
    public class UnlockCharacter : TutorialScenario
    {
        private ScrollRect _battlePassScroll;
        
        [Inject] private ABTest.ABTest _abTest;
        [Inject] private UIRoot _uiRoot;

        private ScrollRect BattlePassScrollRect => _battlePassScroll ??=
                                                   _uiRoot.gameObject.GetComponentInChildren<BattlePassLevelListView>()
                                                       .ScrollRect; 
        
        public override bool IsStartAllowed(string screenUrl)
        {
            return screenUrl == MainScreenPresenter.URL && TutorialService.IsScenarioCompleted(TutorialScenarioId.Launch) && !_abTest.NoUnlockCharacter;
        }
        public override IEnumerator Run()
        {
            yield return ShowPopupStep("UnlockTutorialMessageStepId1");
            yield return ShowPopupStep("UnlockTutorialMessageStepId2");
            yield return ClickUiElementStep("BattlePassWidget");
            
            TutorialService.CompleteScenario(ScenarioId);
            BattlePassScrollRect.vertical = false;
            
            yield return ClickUiElementStep(BattlePassRewardView.GetTutorialId(new BattlePassRewardId(1, BattlePassRewardType.Basic)));
            yield return ClickUiElementStep(BattlePassRewardView.GetTutorialId(new BattlePassRewardId(2, BattlePassRewardType.Basic)));

            BattlePassScrollRect.vertical = true;
            
            yield return ShowPopupStep("UnlockTutorialMessageStepId6");
            yield return ShowPopupStep("UnlockTutorialMessageStepId7");
            yield return ShowPopupStep("UnlockTutorialMessageStepId8");    
            
            yield return ClickUiElementStep(FooterButtonView.GetTutorialId(FooterButtonType.Army), HandDirection.Down);

            yield return ShowPopupStep("UnlockTutorialMessageStepId10");
            yield return ClickUnitButtonStep(UnitButton.GetTutorialId("UnitSniper"));
            
            yield return ShowPopupStep("UnlockTutorialMessageStepId12");
            yield return ClickUiElementStep("CharacterCraftButton");
            
            yield return ShowPopupStep("UnlockTutorialMessageStepId14");
            yield return ShowPopupStep("UnlockTutorialMessageStepId15");   
            yield return ShowPopupStep("UnlockTutorialMessageStepId16");
          
            yield return ClickUiElementStep("CharacterUpgradeButton");
            
            yield return ShowPopupStep("UnlockTutorialMessageStepId18");
            yield return ShowPopupStep("UnlockTutorialMessageStepId19");
            
            yield return ClickUiElementStep(FooterButtonView.GetTutorialId(FooterButtonType.Duel), HandDirection.Down);
            yield return ClickUiElementStep("StartDuelButton");
            CompleteSteps();
            FinishScenario();
        }
        
        private IEnumerator ClickUnitButtonStep(string elementId)
        {
            NextStep();
            yield return new WaitForTutorialElementActivated(elementId);
            
            var uiElement = TutorialUiElementObserver.Get(elementId);
            UiTools.ElementHighlighter.Set(uiElement);
            UiTools.TutorialHand.ShowOnElement(uiElement);
            
            var unitButton = uiElement.GetComponent<UnitButton>();
            unitButton.SetEnabled(true);
            
            yield return new WaitForTutorialElementClicked(elementId);

            UiTools.TutorialHand.Hide();
            UiTools.ElementHighlighter.Clear();
        }
    }
}