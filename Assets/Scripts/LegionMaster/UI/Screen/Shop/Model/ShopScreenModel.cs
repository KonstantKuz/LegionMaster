using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;

namespace LegionMaster.UI.Screen.Shop.Model
{
    public class ShopScreenModel
    {
        public readonly ShopSectionId ActiveSectionId;
        private readonly List<ShopSectionModel> _sectionModels;

        public IReadOnlyCollection<ShopSectionModel> SectionModels => _sectionModels;

        public ShopScreenModel(ShopScreenConfig shopScreenConfig,
                               StringKeyedConfigCollection<ProductConfig> shopConfig,
                               StringKeyedConfigCollection<PurchaseConfig> purchases,
                               PackConfigCollection packs,
                               ShopService shop,
                               InAppPurchaseService inAppPurchaseService,
                               Action<string> onBuyClick,
                               ShopSectionId activeSectionId)
        {
            _sectionModels = shopScreenConfig.SectionConfigs.Select(sectionConfig => new ShopSectionModel(sectionConfig, shopConfig, purchases, packs, shop, inAppPurchaseService, onBuyClick)).ToList();
            ActiveSectionId = activeSectionId;
        }
    }
}