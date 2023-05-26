using LegionMaster.Cheats;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using LegionMaster.UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Cheats
{
    public class CheatsPanelScreenPresenter : MonoBehaviour
    {
   
        [SerializeField] private ActionButton _closeButton;
        [SerializeField] private ActionButton _hideButton;
        
        [SerializeField] private ActionButton _winBattleButton;
        [SerializeField] private ActionButton _loseBattleButton;
        
        [SerializeField] private ActionButton _resetProgressButton;
        
        [SerializeField] private ActionButton _unlockAllUnitsButton;
        [SerializeField] private ActionButton _upgradeAllUnitsButton;

        [SerializeField] private ActionButton _toggleFPSButton;
        [SerializeField] private ActionButton _toggleConsoleButton;
        [SerializeField] private ActionToggle _consoleEnableToggle;

        [SerializeField] private ActionButton _dailyQuestButton;
        [SerializeField] private ActionButton _weeklyQuestButton;

        [SerializeField] private ActionToggle _unitInfoToggle;
        
        [SerializeField] private ActionButton _restartWithNewIdButton; 
        [SerializeField] private ActionButton _analyticsTestEventButton;

        [SerializeField] private InputField _inputField;
        [SerializeField] private ActionButton _setLanguage;

        [SerializeField] private ActionButton _setRussianLanguage;
        [SerializeField] private ActionButton _setEnglishLanguage;
        
        [Inject] private CheatsManager _cheatsManager;
        [Inject] private CheatsActivator _cheatsActivator;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private QuestService _questService;

        
        private void OnEnable()
        {
            _closeButton.Init(HideCheatsPanelScreen);
            _hideButton.Init(DisableCheats);
            
            if (_screenSwitcher.ActiveScreen.ScreenId == ScreenId.Battle)
            {
                _winBattleButton.Init(WinBattle);
                _loseBattleButton.Init(LoseBattle);
                _winBattleButton.Button.interactable = true;
                _loseBattleButton.Button.interactable = true;
            }
            
            _resetProgressButton.Init(ResetProgress);
            
            _unlockAllUnitsButton.Init(UnlockAllUnits);
            _upgradeAllUnitsButton.Init(UpgradeAllUnits);

            _toggleFPSButton.Init(ToggleFPSMonitor);
            _toggleConsoleButton.Init(ToggleDebugConsole);
            _consoleEnableToggle.Init(_cheatsManager.IsConsoleEnabled, value => {
                _cheatsManager.IsConsoleEnabled = value;
            });

            _dailyQuestButton.Init(FinishDailyQuests);
            _weeklyQuestButton.Init(FinishWeeklyQuests);
            
            _unitInfoToggle.Init(_cheatsManager.IsUnitInfoEnabled, value => { _cheatsManager.IsUnitInfoEnabled = value; });
            _restartWithNewIdButton.Init(_cheatsManager.RestartWithNewAnalyticsId);
            _analyticsTestEventButton.Init(_cheatsManager.ReportAnalyticsTestEvent);
            
            _setLanguage.Init(SetLanguage);
            _setEnglishLanguage.Init(SetEnglishLanguage);
            _setRussianLanguage.Init(SetRussianLanguage);
        }

        private void DisableCheats()
        {
            _cheatsActivator.ShowCheatsButtonScreen(false);
            _cheatsActivator.ShowCheatsPanelScreen(false);
            _cheatsActivator.EnableCheats(false);
        }

        private void HideCheatsPanelScreen() => _cheatsActivator.ShowCheatsPanelScreen(false);

        private void WinBattle() => _cheatsManager.WinBattle();

        private void LoseBattle() => _cheatsManager.LoseBattle();

        private void ResetProgress() => _cheatsManager.ResetProgress();

        private void UnlockAllUnits() => _cheatsManager.UnlockAllUnits();

        private void UpgradeAllUnits() => _cheatsManager.UpgradeAllUnits();

        private void ToggleFPSMonitor() => _cheatsManager.ToggleFPSMonitor();

        private void ToggleDebugConsole() => _cheatsManager.ToggleDebugConsole();

        private void FinishDailyQuests() => _questService.TimeoutQuests(QuestSectionType.Daily);

        private void FinishWeeklyQuests() => _questService.TimeoutQuests(QuestSectionType.Weekly);

        private void SetLanguage() => _cheatsManager.SetLanguage(_inputField.text);
        private void SetEnglishLanguage() => _cheatsManager.SetLanguage("English");
        private void SetRussianLanguage() => _cheatsManager.SetLanguage("Russian");
    }
}