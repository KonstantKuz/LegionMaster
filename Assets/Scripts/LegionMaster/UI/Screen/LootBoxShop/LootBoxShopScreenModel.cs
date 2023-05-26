using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.LootBox.Config;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Screen.Description.Model;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class LootBoxShopScreenModel
    {
        private StringKeyedConfigCollection<ProductConfig> _shopConfig;
        private LootBoxShopConfig _lootBoxShopConfig;
        private ShopService _shop;
        private PackConfigCollection _packs;
        
        private Action<string> _onBuyClick;
        private List<LootBoxPlacementModel> _placements;
        public IReadOnlyCollection<LootBoxPlacementModel> Placements => _placements;
        
        public LootBoxShopScreenModel(StringKeyedConfigCollection<ProductConfig> shopConfig, LootBoxShopConfig lootBoxShopConfig, ShopService shop, PackConfigCollection packs, Action<string> onBuyClick)
        {
            _shopConfig = shopConfig;
            _lootBoxShopConfig = lootBoxShopConfig;
            _shop = shop;
            _onBuyClick = onBuyClick;
            _packs = packs;
            _placements = _lootBoxShopConfig.Items.Select(BuildLootBoxPlacement).ToList();
        }
        public void UpdateProducts()
        {
            _placements.ForEach(it => {
                var placementConfig = _lootBoxShopConfig.GetPlacementConfigById(it.PlacementId);
                it.UpdateProducts(placementConfig.Products.Select(BuildProductItemModel));
            });
        }
        private LootBoxPlacementModel BuildLootBoxPlacement(LootBoxPlacementConfig placementConfig)
        {
            return new LootBoxPlacementModel(placementConfig.PlacementId, placementConfig.Products.Select(BuildProductItemModel));
        }

        private ProductItemModel BuildProductItemModel(LootBoxProductConfig lootBoxProduct)
        {
            var productConfig = _shopConfig.Get(lootBoxProduct.ProductId);
            
            var product = ProductItemModel.CreateFromShop(productConfig, _packs, PriceButtonModel.FromProduct(productConfig, _shop), _onBuyClick);
            var rewardCount = productConfig.GetRewards(_packs).First().Count;
            product.CountText = rewardCount > 1 ? $"{rewardCount}x" : "";
            return product;
        }
    }
}