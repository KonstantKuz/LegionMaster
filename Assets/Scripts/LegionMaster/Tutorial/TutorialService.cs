using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Core.Config;
using LegionMaster.Repository;
using LegionMaster.Tutorial.Model;
using LegionMaster.Tutorial.Scenario;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI.Screen;
using UnityEngine;
using Zenject;

namespace LegionMaster.Tutorial
{
    [PublicAPI]
    public class TutorialService : MonoBehaviour
    {
        [SerializeField]
        private List<TutorialScenario> _scenarios;
        [SerializeField]
        private TutorialUiTools _uiTools;

        [Inject]
        private TutorialRepository _repository;
        [Inject]
        private ScreenSwitcher _screenSwitcher;

        private List<TutorialScenario> _activeScenarios = new List<TutorialScenario>();
        public TutorialScenario CurrentScenario { get; private set; }
        
        public void Init()
        {
            if (AppConfig.IsHyperCasual) {
                RegisterScenario(TutorialScenarioId.LaunchHyperCasual);
            } else {
                RegisterScenario(TutorialScenarioId.LaunchCampaign)
                    .RegisterScenario(TutorialScenarioId.SecondUnlockCharacter);
            }
        }

        public TutorialService RegisterScenario(TutorialScenarioId scenarioId)
        {
            var activeScenario = _scenarios.FirstOrDefault(it => it.ScenarioId == scenarioId)
                                 ?? throw new
                                         NullReferenceException($"Register scenario error, tutorial scenario not added, scenario:= {scenarioId}");
            if (_activeScenarios.Contains(activeScenario)) {
                throw new NullReferenceException($"Register scenario error, tutorial scenario already register, scenario:= {scenarioId}");
            }
            _activeScenarios.Add(activeScenario);
            return this;
        }

        private void Awake()
        {
            _screenSwitcher.OnAfterScreenSwitched += OnScreenSwitched;
        }

        private void OnDestroy()
        {
            _screenSwitcher.OnAfterScreenSwitched -= OnScreenSwitched;
        }

        public void StartScenario(TutorialScenarioId scenarioId)
        {
            CurrentScenario = GetScenario(scenarioId);
            StartCoroutine(CurrentScenario.Run());
        }

        public void FinishScenario(TutorialScenarioId scenarioId)
        {
            if (!IsScenarioCompleted(scenarioId)) {
                CompleteScenario(scenarioId);
            }
            if (CurrentScenario.ScenarioId != scenarioId) {
                Debug.LogError($"FinishScenario error, scenarioId= {scenarioId} is not equal to currentScenario = {CurrentScenario.ScenarioId}");
                return;
            }
            CurrentScenario = null;
        }

        public bool IsScenarioCompleted(TutorialScenarioId scenarioId)
        {
            var scenarios = State.Scenarios;
            return scenarios.ContainsKey(scenarioId) && scenarios[scenarioId];
        }

        public void CompleteScenario(TutorialScenarioId scenarioId)
        {
            var state = State;
            state.Scenarios[scenarioId] = true;
            _repository.Set(state);
        }

        public void ResetCurrentScenario() => CurrentScenario = null;

        private void OnScreenSwitched(string screenUrl)
        {
            if (!AppConfig.TutorialEnabled) {
                return;
            }
            TryStartTutorial(screenUrl);
        }

        private void TryStartTutorial(string screenUrl)
        {
            if (CurrentScenario != null) {
                return;
            }
            var scenario = _activeScenarios.FirstOrDefault(it => !IsScenarioCompleted(it.ScenarioId) && it.IsStartAllowed(screenUrl));
            if (scenario == null) {
                return;
            }
            StartScenario(scenario.ScenarioId);
        }

        public bool AllScenarioCompleted => _activeScenarios.All(it => IsScenarioCompleted(it.ScenarioId));
        public bool IsRunning => CurrentScenario != null;
        private TutorialState State => _repository.Get() ?? new TutorialState();

        private TutorialScenario GetScenario(TutorialScenarioId scenarioId) =>
                _activeScenarios.FirstOrDefault(it => it.ScenarioId == scenarioId)
                ?? throw new NullReferenceException($"Tutorial scenario not registered, scenario:= {scenarioId}");

        public TutorialUiTools UiTools => _uiTools;
    }
}