using System;
using LegionMaster.Analytics.Data;
using LegionMaster.Analytics.Wrapper;
using LegionMaster.Analytics.Wrapper.GameAnalytics;
using LegionMaster.Localization.Service;
using LegionMaster.Location.Session.Service;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.Repository;
using LegionMaster.Units.Component;
using LegionMaster.Units.Config;
using LegionMaster.UpgradeUnit.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Cheats
{
    public class CheatsManager : MonoBehaviour
    {
        private const string ACQUISITION_PLACE = "Cheats";

        [Inject] private PlayerProgressService _progressService;
        [Inject] private WalletService _walletService;
        [Inject] private InventoryService _inventoryService;
        [Inject] private PlayerSquadService _playerSquadService;
        [Inject] private Analytics.Analytics _analytics;
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private UpgradeUnitService _upgradeUnitService;
        [Inject] private DiContainer _container;
        [Inject] private LocalizationService _localizationService;
        
        [SerializeField] private GameObject _fpsMonitor;
        [SerializeField] private GameObject _debugConsole;

        private readonly CheatRepository _repository = new CheatRepository();
        private void Awake()
        {
            _debugConsole.SetActive(IsConsoleEnabled);
#if DEBUG_CONSOLE_ENABLED
            _debugConsole.SetActive(true);
#endif
        }

        public void WinBattle() => BattleSessionService.FinishBattleWithCheats(UnitType.PLAYER);
        public void LoseBattle() => BattleSessionService.FinishBattleWithCheats(UnitType.AI);
        
        public void ResetProgress()
        {
            _progressService.ResetProgress();
            _walletService.ResetWallet();
            _inventoryService.ResetInventory();
            _playerSquadService.ResetAllSquads();
            PlayerPrefs.DeleteAll();
        }

        public void AddCurrency(Currency currency, int amount)
        {
            using (_analytics.SetAcquisitionProperties(ACQUISITION_PLACE, ResourceAcquisitionType.Cheats))
            {
                _walletService.Add(currency, amount);
            }
        }

        public void AddExp(int amount)
        {
            using (_analytics.SetAcquisitionProperties(ACQUISITION_PLACE, ResourceAcquisitionType.Cheats))
            {
                _progressService.AddExp(amount);
            }
        }

        public void UnlockAllUnits()
        {
            foreach (var unitId in _unitCollectionConfig.AllUnitIds)
            {
                _inventoryService.LoadUnit(unitId, 1, _unitCollectionConfig.GetUnitStartingStars(unitId));
            }
        }
        
        public void UpgradeAllUnits()
        {
            foreach (var unitId in _unitCollectionConfig.AllUnitIds)
            {
                if (!_inventoryService.ContainsUnit(unitId)) continue;
                _upgradeUnitService.UpgradeToPlayerLevel(unitId);
            }
        }

        public void AddFragments(string unitId, int amount)
        {
            using (_analytics.SetAcquisitionProperties(ACQUISITION_PLACE, ResourceAcquisitionType.Cheats))
            {
                _inventoryService.AddUnitFragments(unitId, amount);
            }
        }
        public void RestartWithNewAnalyticsId()
        {
            GameAnalyticsId.GenerateNewIdOnNextStart();
            Application.Quit();
        }  
        public void ReportAnalyticsTestEvent()
        {
            _analytics.ReportTest();
        }

        public void ToggleFPSMonitor() => _fpsMonitor.SetActive(!_fpsMonitor.activeInHierarchy);

        public void ToggleDebugConsole() => _debugConsole.SetActive(!_debugConsole.activeInHierarchy);

        public bool IsConsoleEnabled
        {
            get => Settings.ConsoleEnabled;
            set
            {
                UpdateSettings(settings => { settings.ConsoleEnabled = value; });
            }
        }
        
        public bool IsUnitInfoEnabled
        {
            get => Settings.UnitInfoEnabled;
            set {
                UpdateSettings(settings => { settings.UnitInfoEnabled = value; });
            }
        }

        private void UpdateSettings(Action<CheatSettings> updateFunc)
        {
            var settings = Settings;
            updateFunc?.Invoke(settings);
            _repository.Set(settings);
        }


        private CheatSettings Settings => _repository.Get() ?? new CheatSettings();
        private IBattleSessionService BattleSessionService => _container.Resolve<IBattleSessionService>();

        public void SetLanguage(string language)
        {
            _localizationService.SetLanguageOverride(language);
        }
    }
}

