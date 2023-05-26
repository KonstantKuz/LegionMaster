using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Analytics.Event;
using LegionMaster.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Progress.Config;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Repository;
using LegionMaster.UpgradeUnit.Config;
using LegionMaster.UpgradeUnit.Message;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.UpgradeUnit.Service
{
    [UsedImplicitly]
    public class UpgradeUnitService
    {
        [Inject]
        private StringKeyedConfigCollection<UpgradeLevelConfig> _upgradeLevelStringKeyedConfig;
        [Inject]
        private InventoryRepository _inventoryRepository;
        [Inject]
        private PlayerProgressService _playerProgressService;
        [Inject]
        private Analytics.Analytics _analytics;
        [Inject]
        private IMessenger _messenger;
        [Inject]
        private StringKeyedConfigCollection<LevelConfig> _playerLevels;

        public int GetCurrencyAmountByLevel(string unitId, int level)
        {
            return GetLevelCostIncrease(unitId);
        }

        public bool CanUpgradeNextLevel(string unitId)
        {
            return CanUpgradeLevel(Inventory.GetUnit(unitId).Level + 1);
        }

        private bool CanUpgradeLevel(int level)
        {
            return level <= _playerProgressService.Progress.MaxPlayerLevel(_playerLevels);
        }

        public void UpgradeNextLevel(string unitId)
        {
            var unit = Inventory.GetUnit(unitId);
            var nextLevel = unit.Level + 1;
            if (!CanUpgradeLevel(nextLevel)) {
                throw new Exception($"Upgrade level more than level player, upgradeLevel:= {nextLevel}, playerLevel= {PlayerLevel}");
            }

            var upgradePrice = GetCurrencyAmountByLevel(unitId, unit.Level);
            if (unit.Fragments < upgradePrice)
            {
                throw new Exception(
                    $"Not enough fragments {unit.Fragments} (required {upgradePrice}) to upgrade unit {unitId} to level {nextLevel}");
            }
            var inventory = Inventory;
            unit.Fragments -= upgradePrice;
            unit.Level = nextLevel;
            _inventoryRepository.Set(inventory);
            ReportUnitUpgrade(unitId, unit);
            _messenger.Publish(new UnitUpgradeMessage());
        }

        private void ReportUnitUpgrade(string unitId, InventoryUnit unit)
        {
            _analytics.ReportUnitEvent(unitId, UnitEvents.UPGRADED, new Dictionary<string, object>
            {
                { $"level{unit.Level}", $"progress{_playerProgressService.Progress.TotalProgress}" }
            });
        }

        public void UpgradeToPlayerLevel(string unitId)
        {
            var unit = Inventory.GetUnit(unitId);
            var nextLevel = PlayerLevel;
            var inventory = Inventory;
            unit.Level = nextLevel;
            _inventoryRepository.Set(inventory);
            ReportUnitUpgrade(unitId, unit);
            _messenger.Publish(new UnitUpgradeMessage());
        }

        private int GetLevelCostIncrease(string unitId)
        {
            return _upgradeLevelStringKeyedConfig.Get(unitId).LevelCostIncrease;
        }

        private int PlayerLevel => _playerProgressService.Progress.Level;
        private Inventory Inventory => _inventoryRepository.Require();
    }
}