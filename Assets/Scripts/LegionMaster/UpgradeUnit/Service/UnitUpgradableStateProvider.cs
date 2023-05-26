using JetBrains.Annotations;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Model;
using Zenject;

namespace LegionMaster.UpgradeUnit.Service
{
    [PublicAPI]
    public class UnitUpgradableStateProvider
    {
        [Inject] private InventoryService _inventoryService;
        [Inject] private UpgradeUnitService _upgradeUnitService;

        public UnitUpgradableState Get(string unitId)
        {
            var unit = _inventoryService.FindUnit(unitId);
            return unit == null ? GetLockedUnitState(unitId) : Get(unit);
        }

        public UnitUpgradableState Get(UnitModel unit)
        {
            if (!unit.InventoryUnit.IsUnlocked)
            {
                return GetLockedUnitState(unit.UnitId, unit.InventoryUnit.Fragments);
            }

            var levelUpgradePrice = _upgradeUnitService.GetCurrencyAmountByLevel(unit.UnitId, unit.RankedUnit.Level);

            return new UnitUpgradableState
            {
                IsUnlocked = true,
                CanUpgradeLevel = unit.InventoryUnit.Fragments >= levelUpgradePrice &&
                                  _upgradeUnitService.CanUpgradeNextLevel(unit.UnitId),
                LevelUpgradePrice = levelUpgradePrice
            };
        }

        private UnitUpgradableState GetLockedUnitState(string unitId, int fragments = 0)
        {
            var price = _inventoryService.GetFragmentsToUnlock(unitId);
            return new UnitUpgradableState
            {
                CanCraft = fragments >= price,
                CraftPrice = price
            };
        }
    }
}