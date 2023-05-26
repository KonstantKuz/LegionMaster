using System.Collections;
using System.Linq;
using LegionMaster.BattlePass.Model;
using LegionMaster.Extension;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Tutorial.UI;
using LegionMaster.Tutorial.WaitConditions;
using LegionMaster.UI.Screen;
using LegionMaster.UI.Screen.BattlePass;
using LegionMaster.UI.Screen.BattlePass.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.Tutorial.ScreenTutorial
{
    public class BattlePassRewardTutorial : MonoBehaviour
    {
        private const int MAX_MATCHES_FOR_START_TUTORIAL = 10;

        [SerializeField] private BattlePassScreenPresenter _battlePassPresenter;
        [SerializeField] private ScrollRect _battlePassScroll;
        [SerializeField] private float _scrollDuration = 0.5f;

        [Inject] private TutorialService _tutorialService;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private PlayerProgressService _playerProgress;

        private TutorialUiTools UiTools => _tutorialService.UiTools;

        private void Awake() => _screenSwitcher.OnAfterScreenSwitched += OnAfterScreenSwitched;

        private void OnDestroy() => _screenSwitcher.OnAfterScreenSwitched -= OnAfterScreenSwitched;

        private void OnAfterScreenSwitched(string screenUrl) => TryStartTutorial(screenUrl);

        private void OnDisable()
        {
            UiTools.HideOverlay();
            UiTools.TutorialHand.Hide();
            UiTools.ElementHighlighter.Clear();
        }

        private void TryStartTutorial(string screenUrl)
        {
            if (!IsStartAllowed(screenUrl)) {
                return;
            }
            StartCoroutine(Run());
        }

        private bool IsStartAllowed(string screenUrl)
        {
            // todo: synchronize with tutorial scenarios
            return screenUrl == BattlePassScreenPresenter.URL
                   && !_tutorialService.IsRunning
                   && _tutorialService.AllScenarioCompleted
                   && !_battlePassPresenter.Model.FromMenu 
                   && _playerProgress.Progress.TotalProgress <= MAX_MATCHES_FOR_START_TUTORIAL;
        }

        private IEnumerator Run()
        {
            var availableRewardIds = _battlePassPresenter.Model.LevelListModel.Levels
                                                         .SelectMany(it => new[] {
                                                                 it.Value.BasicReward.BattlePassReward, 
                                                                 it.Value.PremiumReward.BattlePassReward
                                                         })
                                                         .Where(it => it.State == BattlePassRewardState.Available)
                                                         .Select(it => it.Id);

            _battlePassScroll.vertical = false;
            foreach (var rewardId in availableRewardIds) {
                yield return ClickRewardStep(BattlePassRewardView.GetTutorialId(rewardId), rewardId.Level != 1);
            }
            _battlePassScroll.vertical = true;
        }

        private IEnumerator ClickRewardStep(string elementId, bool scrollToReward)
        {
            UiTools.ShowOverlay();
            yield return new WaitForTutorialElementActivated(elementId);

            var uiElement = TutorialUiElementObserver.Get(elementId);
            UiTools.ElementHighlighter.Set(uiElement);

            if (scrollToReward) {
                var scrollChildTransform = uiElement.GetComponentInParent<BattlePassLevelView>().GetComponent<RectTransform>();
                yield return _battlePassScroll.ScrollToChildCoroutine(scrollChildTransform, _scrollDuration);
            }
            UiTools.TutorialHand.ShowOnElement(uiElement);
            UiTools.HideOverlay();

            yield return new WaitForTutorialElementClicked(elementId);
            UiTools.TutorialHand.Hide();
            UiTools.ElementHighlighter.Clear();
        }
    }
}