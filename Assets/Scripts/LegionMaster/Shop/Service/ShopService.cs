using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Service;
using LegionMaster.Config;
using LegionMaster.LootBox.Model;
using LegionMaster.Player.Inventory.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Model;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Message;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.Shop.Service
{
    public class ShopService
    {
        [Inject] private WalletService _walletService;
        [Inject] private StringKeyedConfigCollection<ProductConfig> _shopConfig;
        [Inject] private StringKeyedConfigCollection<ResourceConfig> _resourceCollectionStringKeyedConfig; 
        [Inject] private BattlePassService _battlePassService;
        [Inject] private PackConfigCollection _packs;
        [Inject] private PlayerProgressService _playerProgress;
        [Inject] private Analytics.Analytics _analytics;    
        [Inject] private IMessenger _messenger;
     
        [CanBeNull]
        public IEnumerable<RewardItem> TryBuy(string productId)
        {
            var product = GetProductById(productId);
            if (!_walletService.TryRemove(product.Currency, product.CurrencyCount)) {
                return null;
            }
            _messenger.Publish(new ProductBuyingMessage(productId));
            return product.GetRewards(_packs);
        }
        public bool HasEnoughCurrency(string productId)
        {
            var product = GetProductById(productId);
            return _walletService.HasEnoughCurrency(product.Currency, product.CurrencyCount);
        }
        public IObservable<bool> HasEnoughCurrencyAsObservable(string productId)
        {
            var product = GetProductById(productId);
            return _walletService.HasEnoughCurrencyAsObservable(product.Currency, product.CurrencyCount);
        }
        public ProductConfig GetProductById(string productId) => _shopConfig.Get(productId);
        
        [CanBeNull]
        public LootBoxModel TryBuyLootBox(string productId)
        {
            var lootBoxReward = TryBuy(productId).First();
            if (lootBoxReward == null) return null;
            var lootBox = LootBoxModel.FromReward(lootBoxReward);
            
            _playerProgress.IncreaseCollectiblesCount(LootBoxToCollectibles(lootBox));
            _analytics.ReportResourceGained(CurrencyType.Chest, lootBoxReward.Count);
            return lootBox;
        }
 
        [CanBeNull]
        public RewardItem TryBuyBattlePassExp()
        {
            var currencyCount = GetBattlePassExpPrice();
            var amountExp = _battlePassService.GetNeededExpUntilNextLevel;
            if (!_walletService.TryRemove(Currency.Hard, currencyCount)) {
                return null;
            }
            return new RewardItem(RewardType.BattlePassExp.ToString(), RewardType.BattlePassExp, amountExp);
        }
        public bool HasEnoughCurrencyForBattlePassExp()
        {
            return _walletService.HasEnoughCurrency(Currency.Hard, GetBattlePassExpPrice());
        }
        public int GetBattlePassExpPrice()
        {
            double neededExp = _battlePassService.GetNeededExpUntilNextLevel;
            double expValue = _resourceCollectionStringKeyedConfig.Get(Resource.BattlePassExp.ToString()).Value;     
            double hardValue = _resourceCollectionStringKeyedConfig.Get(Resource.Hard.ToString()).Value;
            return (int) Math.Ceiling(neededExp * expValue / hardValue);
        }
        private PlayerCollectibles LootBoxToCollectibles(LootBoxModel lootBox)
        {
            return lootBox.Id switch {
                    LootBoxId.LootBoxCommon => PlayerCollectibles.LootBoxCommon,
                    LootBoxId.LootBoxRare => PlayerCollectibles.LootBoxRare,
                    _ => PlayerCollectibles.None
            };
        }
    }
}