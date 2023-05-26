using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Store.Config;
using LegionMaster.Extension;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Units.Model.Meta;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace LegionMaster.Duel.Store
{
    public class DuelUnitStoreService
    {
        [Inject]
        public InventoryService _inventoryService;
        [Inject]
        private StringKeyedConfigCollection<DuelProductConfig> _duelShopStringKeyedConfig;     
        [Inject]
        private StringKeyedConfigCollection<UnitRarityConfig> _unitsRarityConfig;
        [Inject]
        private WalletService _walletService;
        [Inject]
        private DuelConfig _duelConfig;
        
        private Dictionary<string, int> _unitsStock;
        public IObservable<bool> IsStockEmptyAsObservable => _unitsStock.ObserveEveryValueChanged(it => it.Values.All(it => it <= 0));
        public void Init()
        {
            _unitsStock = BuildStock(_inventoryService);
        }
        public static Dictionary<string, int> BuildStock(InventoryService inventoryService)
        {
            return inventoryService.UnlockedUnits.ToDictionary(it => it.UnitId, GetUnitCount);
        }
        public int GetUnitPrice(string unitId) => _duelShopStringKeyedConfig.Get(unitId).Price;

        public bool CanBuyUnit(string unitId) => _walletService.GetMoney(Currency.DuelToken) >= GetUnitPrice(unitId);

        public bool TryBuyUnit(string unitId) => _walletService.TryRemove(Currency.DuelToken, GetUnitPrice(unitId));

        public bool TryBuyShopUpdate() => !IsStockEmpty(_unitsStock) && _walletService.TryRemove(Currency.DuelToken, _duelConfig.ShopUpdatePrice);
        
        public IEnumerable<string> AcquireUnits(int amount)
        {
            return GetRandomUnitsIdByWeight(amount, _unitsStock, _unitsRarityConfig);
        }
        public void ReturnUnits(IEnumerable<string> unitIds)
        {
            unitIds.ForEach(id => _unitsStock[id]++);
        }
        public static IEnumerable<string> GetRandomUnitsIdByWeight(int amount, Dictionary<string, int> unitsStock, StringKeyedConfigCollection<UnitRarityConfig> unitsRarityConfig)
        {
            for (int i = 0; i < amount; i++) {
                if (IsStockEmpty(unitsStock)) {
                    yield break;
                }
                var unitId = GetRandomUnitIdByWeight(unitsStock, unitsRarityConfig);
                unitsStock[unitId]--;
                yield return unitId;
            }
        }
        public static bool IsStockEmpty(Dictionary<string, int> unitsStock) => unitsStock.Count == 0 || unitsStock.Values.All(it => it <= 0);
        public static string GetRandomUnitIdByWeight(Dictionary<string, int> unitsStock, StringKeyedConfigCollection<UnitRarityConfig> unitsRarityConfig)
        {
            var unitWeights = unitsStock.Where(unitStock => unitStock.Value > 0)
                                        .ToDictionary(unitStock => unitStock.Key,
                                                      unitStock => unitsRarityConfig.Get(unitStock.Key).RarityWeight * unitStock.Value);
            int sumWeight = unitWeights.Values.Sum();
            int probability = Random.Range(1, sumWeight + 1);
            foreach (var weight in unitWeights) {
                if (weight.Value >= probability) {
                    return weight.Key;
                }
                probability -= weight.Value;
            }
            Debug.LogWarning("generation error by weight");
            return unitWeights.Keys.ToList().Random();
        }
        private static int GetUnitCount(UnitModel unit) => (int) Math.Pow(2, unit.RankedUnit.Star);
        
  
    }
}