using LegionMaster.Core.Loadable;
using LegionMaster.Tutorial;
using LegionMaster.Tutorial.Model;
using Zenject;

namespace LegionMaster.ABTest.Loadable
{
    public class ABTestLoadable : AppLoadable
    {
        [Inject] 
        private ABTest _abTest;
        [Inject] 
        private Analytics.Analytics _analytics;
        [Inject(Optional = true)] 
        private TutorialService _tutorialService;

        private IABTestProvider _abTestProvider;
        protected override void Run()
        {
            _abTestProvider = new GameAnalyticsABTestProvider(_abTest);
            _abTestProvider.OnLoaded += OnLoadedABTest;
            _abTestProvider.Load();
            _analytics.Init();
            Next();
        }
        private void OnLoadedABTest()
        {
            _abTestProvider.OnLoaded -= OnLoadedABTest;
            if (_abTest.NoUnlockCharacter && _tutorialService != null) {
                _tutorialService.CompleteScenario(TutorialScenarioId.SecondUnlockCharacter);
            }
        }
    }
}