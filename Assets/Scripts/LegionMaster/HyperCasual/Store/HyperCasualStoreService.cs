using System;
using LegionMaster.HyperCasual.Store.Config;
using LegionMaster.HyperCasual.Store.Data;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Repository;
using Zenject;

namespace LegionMaster.HyperCasual.Store
{
    public class HyperCasualStoreService
    {
        [Inject]
        public HyperCasualPurchasedUnitsRepository _repository;
        [Inject]
        private HyperCasualSettingsConfig _hyperCasualSettingsConfig;
        [Inject]
        private WalletService _walletService;
        
        public bool TryBuy(MergeableUnitType product)
        {
            var price = CalculatePrice(product);
            if (!_walletService.TryRemove(Currency.Soft, price)) {
                return false;
            }
            IncreasePurchasedUnits(product);
            return true;
        }
        public int CalculatePrice(MergeableUnitType product)
        {
            var purchaseCount = PurchasedUnits.GetCount(product);
            var initialUnitCost = _hyperCasualSettingsConfig.GetInitialUnitCost(product);
            return initialUnitCost + (int) (initialUnitCost * _hyperCasualSettingsConfig.CostFactor * purchaseCount);
        }
        public IObservable<bool> HasEnoughCurrencyAsObservable(MergeableUnitType product)
        {
            return _walletService.HasEnoughCurrencyAsObservable(Currency.Soft, CalculatePrice(product));
        }
        private void IncreasePurchasedUnits(MergeableUnitType product)
        {
            var purchasedUnits = PurchasedUnits;
            purchasedUnits.Increase(product);
            _repository.Set(purchasedUnits);
        }
        private PurchasedUnits PurchasedUnits => _repository.Get() ?? new PurchasedUnits();
    }
}