using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Duel.Store;
using LegionMaster.Duel.Store.Config;
using LegionMaster.Player.Inventory.Service;
using SuperMaxim.Core.Extensions;
using Zenject;

namespace LegionMaster.Campaign.Store
{
    public class CampaignUnitStoreService
    {
        [Inject]
        public InventoryService _inventoryService;
        [Inject]
        private StringKeyedConfigCollection<UnitRarityConfig> _unitsRarityConfig;
        
        private Dictionary<string, int> _unitsStock;
        
        public void Init()
        {
            _unitsStock = DuelUnitStoreService.BuildStock(_inventoryService);
        }
        public int UnitAmount => _unitsStock.Values.Where(it => it > 0).Sum();
        public IEnumerable<string> AcquireUnits(List<string> usedUnitsIds, int amount)
        {
            var usedUnits = new List<string>(usedUnitsIds);
            for (int i = 0; i < amount; i++) {
                var unusedUnits = GetUnusedUnitsInStock(usedUnits);
                if (IsStockEmpty(unusedUnits)) {
                    foreach (var randomUnit in GetRandomUnitsIdByWeight(amount - i)) {
                        yield return randomUnit;
                    }
                    break;
                }
                var unitId = DuelUnitStoreService.GetRandomUnitIdByWeight(unusedUnits, _unitsRarityConfig);
                usedUnits.Add(unitId);
                _unitsStock[unitId]--;
                yield return unitId;
            }
        }
        public void ReturnUnits(IEnumerable<string> unitIds)
        {
            unitIds.ForEach(id => _unitsStock[id]++);
        }
        private IEnumerable<string> GetRandomUnitsIdByWeight(int amount) =>
                DuelUnitStoreService.GetRandomUnitsIdByWeight(amount, _unitsStock, _unitsRarityConfig);
        
     
        private Dictionary<string, int> GetUnusedUnitsInStock(List<string> usedUnits) =>
                _unitsStock.Where(it => !usedUnits.Contains(it.Key)).ToDictionary(it => it.Key, it => it.Value);

        private bool IsStockEmpty(Dictionary<string, int> unitsStock) => DuelUnitStoreService.IsStockEmpty(unitsStock);
    }
}