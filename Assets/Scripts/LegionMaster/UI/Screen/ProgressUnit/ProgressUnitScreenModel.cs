using LegionMaster.Player.Inventory.Service;
using LegionMaster.UpgradeUnit.Service;
using LegionMaster.Util;

namespace LegionMaster.UI.Screen.ProgressUnit
{
    public class ProgressUnitScreenModel
    {
        public readonly string UnitId;
        public readonly string UnitIconPath;     
        public readonly string FragmentIconPath;   
        public readonly float FragmentsProgress;
        public readonly string FragmentsProgressLabel;
        public readonly bool UpgradeAvailable;   
 
        public ProgressUnitScreenModel(string unitId, InventoryService inventoryService, UpgradeUnitService upgradeUnitService)
        {
            var unitModel = inventoryService.GetUnit(unitId);
            var fragmentsPrice = inventoryService.IsUnitUnlocked(unitId) ? 
                upgradeUnitService.GetCurrencyAmountByLevel(unitId, unitModel.RankedUnit.Level) : 
                inventoryService.GetFragmentsToUnlock(unitId);
            
            UnitId = unitId;
            UnitIconPath = IconPath.GetUnit(unitId);
            FragmentIconPath = IconPath.GetCurrency(unitId);
            FragmentsProgress = 1.0f * unitModel.InventoryUnit.Fragments / fragmentsPrice;
            FragmentsProgressLabel = $"{unitModel.InventoryUnit.Fragments}/{fragmentsPrice}";
            UpgradeAvailable = upgradeUnitService.CanUpgradeNextLevel(unitId) && unitModel.InventoryUnit.Fragments >= fragmentsPrice;
        }
        
    }
}