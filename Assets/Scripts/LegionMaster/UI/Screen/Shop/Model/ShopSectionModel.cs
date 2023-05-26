using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.LootBoxShop;

namespace LegionMaster.UI.Screen.Shop.Model
{
    public class ShopSectionModel
    {
        public readonly string SectionId;

        private readonly List<ShopProductModel> _products;
        public IReadOnlyCollection<ShopProductModel> Products => _products;

        public ShopSectionModel(ShopSectionConfig sectionConfig, StringKeyedConfigCollection<ProductConfig> shopConfig, StringKeyedConfigCollection<PurchaseConfig> purchases,
                                PackConfigCollection packs, ShopService shop, InAppPurchaseService inAppPurchaseService,
                                Action<string> onBuyClick)
        {
            SectionId = sectionConfig.SectionId;
            _products = sectionConfig.Products.Select(shopViewProduct => BuildShopProduct(shopViewProduct,
                                                                                          shopConfig, purchases, packs, shop, inAppPurchaseService,
                                                                                          onBuyClick))
                .Where(it => it != null)
                .ToList();
        }

        private static ShopProductModel BuildShopProduct(ShopProductConfig shopViewProduct, StringKeyedConfigCollection<ProductConfig> shopStringKeyedConfig, StringKeyedConfigCollection<PurchaseConfig> purchases,
                                                  PackConfigCollection packs, ShopService shop, InAppPurchaseService inAppPurchaseService,
                                                  Action<string> onBuyClick)
        {
            var productId = shopViewProduct.ProductId;
            var isInApp = !shopStringKeyedConfig.Contains(productId);
            if (isInApp && !inAppPurchaseService.IsBillingInitialized) return null;
            var product = isInApp ? BuildProductFromPurchase(productId, purchases, packs, inAppPurchaseService, onBuyClick)
                                  : BuildProductFromShop(productId, shopStringKeyedConfig, packs, shop, onBuyClick);

            return new ShopProductModel {
                    ViewPrefabId = shopViewProduct.ViewPrefabId,
                    ViewType = shopViewProduct.ViewType,
                    Bonus = shopViewProduct.Bonus,
                    Product = product,
            };
        }

        private static ProductItemModel BuildProductFromShop(string productId, StringKeyedConfigCollection<ProductConfig> shopStringKeyedConfig, PackConfigCollection packs, 
                                                      ShopService shop, Action<string> onBuyClick)
        {
            var product = shopStringKeyedConfig.Get(productId);
            var priceButton = PriceButtonModel.FromProduct(product, shop);
            return ProductItemModel.CreateFromShop(product, packs, priceButton, onBuyClick);
        }

        private static ProductItemModel BuildProductFromPurchase(string productId, StringKeyedConfigCollection<PurchaseConfig> purchases, 
                                                          PackConfigCollection packs, InAppPurchaseService inAppPurchaseService, 
                                                          Action<string> onBuyClick)
        {
            var purchase = purchases.Get(productId);
            var priceButton = PriceButtonModel.FromPurchase(inAppPurchaseService.GetProduct(productId));
            return ProductItemModel.CreateFromPurchase(purchase, packs, priceButton, onBuyClick);
        }
    }
}