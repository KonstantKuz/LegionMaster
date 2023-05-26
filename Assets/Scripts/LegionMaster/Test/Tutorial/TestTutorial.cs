using System.Collections;
using LegionMaster.Tutorial.Scenario;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using UnityEngine;

namespace LegionMaster.Test.Tutorial
{
    public class TestTutorial : TutorialScenario
    {
        public override bool IsStartAllowed(string screenUrl)
        {
            return !Completed;
        }
        public override IEnumerator Run()
        {
            yield return new WaitForTutorialElementActivated("tutorialTestButton");
            UiTools.TutorialPopup.Show("Start battle");
            
            yield return new WaitForSeconds(4f);
            UiTools.TutorialPopup.Hide();
            var button = TutorialUiElementObserver.Get("tutorialTestButton");
            UiTools.ElementHighlighter.Set(button.gameObject);
            UiTools.TutorialHand.ShowOnElement(button.PointerPosition);
            
            yield return new WaitForTutorialElementClicked("tutorialTestButton");
            UiTools.ElementHighlighter.Clear();
            UiTools.TutorialHand.Hide();
            UiTools.TutorialPopup.Show("Level Completed!");
            
            yield return new WaitForSeconds(4f);
            UiTools.TutorialPopup.Hide();
            gameObject.SetActive(false);
            FinishScenario();
        }
    }
}