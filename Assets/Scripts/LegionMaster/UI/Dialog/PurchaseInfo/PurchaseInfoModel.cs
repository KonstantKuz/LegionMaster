using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Data;
using LegionMaster.Purchase.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Model;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Description.Model;

namespace LegionMaster.UI.Dialog.PurchaseInfo
{
    public class PurchaseInfoModel
    {
        private readonly string _productId;
        private readonly Action<string> _onBuyClick;

        private List<RewardItemModel> _rewards;
        public PriceButtonModel PriceButton;
        public Action OnBuyClick => delegate { _onBuyClick.Invoke(_productId); };
        public IReadOnlyList<RewardItemModel> Rewards => _rewards;
        public string LabelId { get; }

        public PurchaseInfoModel(string productId,
                                 StringKeyedConfigCollection<ProductConfig> shopConfig,
                                 PackConfigCollection packs,
                                 ShopService shop,
                                 Action<string> onBuy)
        {
            _productId = productId;
            LabelId = productId;
            _onBuyClick = onBuy;
            CreateFromProduct(shopConfig.Get(productId), packs, shop);
        }

        public PurchaseInfoModel(string productId,
                                 StringKeyedConfigCollection<PurchaseConfig> purchases,
                                 PackConfigCollection packs,
                                 InAppPurchaseService inAppPurchaseService,
                                 Action<string> onBuy)
        {
            _productId = productId;
            LabelId = productId;
            _onBuyClick = onBuy;
            CreateFromPurchase(purchases.Get(productId), packs, inAppPurchaseService.GetProduct(productId));
        }

        private void CreateFromProduct(ProductConfig product, PackConfigCollection packs, ShopService shop)
        {
            _rewards = BuildRewards(product.GetRewards(packs));
            PriceButton = PriceButtonModel.FromProduct(product, shop);
        }

        private void CreateFromPurchase(PurchaseConfig purchase, PackConfigCollection packs, BillingProductModel billingProductModel)
        {
            _rewards = BuildRewards(purchase.GetRewards(packs));
            PriceButton = PriceButtonModel.FromPurchase(billingProductModel);
        }

        private List<RewardItemModel> BuildRewards(IEnumerable<RewardItem> rewards)
        {
            return rewards.Select(reward => RewardItemModel.Create("", "", reward)).ToList();
        }
    }
}