using System.Collections;
using LegionMaster.Analytics.Data;
using LegionMaster.Tutorial.Model;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using UnityEngine;
using Zenject;

namespace LegionMaster.Tutorial.Scenario
{
    public abstract class TutorialScenario : MonoBehaviour
    {
        [SerializeField]
        private TutorialScenarioId _scenarioId;
        
        [Inject]
        protected TutorialService TutorialService { get; }
        
        [Inject] 
        protected Analytics.Analytics Analytics { get; }
        
        public int CurrentStepId { get; protected set; }
        
        public TutorialScenarioId ScenarioId => _scenarioId;
        
        public abstract bool IsStartAllowed(string screenUrl);

        public abstract IEnumerator Run();
        protected bool Completed => TutorialService.IsScenarioCompleted(_scenarioId);
        
        protected void FinishScenario() => TutorialService.FinishScenario(_scenarioId);
      
        protected bool IsRunning => TutorialService.CurrentScenario == this;

        protected TutorialUiTools UiTools => TutorialService.UiTools;

        protected IEnumerator ShowPopupStep(string localizationId)
        {
            NextStep();
            UiTools.ShowOverlay();
            UiTools.TutorialPopup.Show(localizationId);
            yield return UiTools.WaitForClick();
            UiTools.TutorialPopup.Hide();
            UiTools.HideOverlay();
        }      
        protected void ShowPopupWithoutNpcStep(string localizationId, Transform messagePosition)
        {
            NextStep();
            UiTools.TutorialPopup.Show(localizationId, messagePosition.position, false);
        }

        protected IEnumerator ClickUiElementStep(string elementId, 
            HandDirection direction = HandDirection.Up,
            bool highlight = true,
            bool showBackground = true)
        {
            NextStep();
            yield return new WaitForTutorialElementActivated(elementId);

            var uiElement = TutorialUiElementObserver.Get(elementId);
            if (highlight) {
                UiTools.ElementHighlighter.Set(uiElement, showBackground);
            }
            UiTools.TutorialHand.ShowOnElement(uiElement, direction);

            yield return new WaitForTutorialElementClicked(elementId);

            UiTools.TutorialHand.Hide();
            UiTools.ElementHighlighter.Clear();
        }
        protected void CompleteSteps()
        {
            Analytics.ReportTutorialStep(ScenarioId.ToString(), CurrentStepId, TutorialStepState.End);
        }
        protected void NextStep()
        {
            if (CurrentStepId > 0) {
                Analytics.ReportTutorialStep(ScenarioId.ToString(), CurrentStepId, TutorialStepState.End);
            }
            CurrentStepId++;
            Analytics.ReportTutorialStep(ScenarioId.ToString(), CurrentStepId, TutorialStepState.Start);
        }
    }
}