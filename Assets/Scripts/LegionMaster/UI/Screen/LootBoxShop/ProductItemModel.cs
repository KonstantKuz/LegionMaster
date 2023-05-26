using System;
using System.Linq;
using LegionMaster.Purchase.Config;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.UI.Screen.Description.Model;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class ProductItemModel
    {
        public string ProductId;
        public string CountText;
        public string IconPath;
        public PriceButtonModel PriceButton;
        public Action OnBuyClick;
        public static ProductItemModel CreateFromPurchase(PurchaseConfig purchase, 
                                                          PackConfigCollection packs, 
                                                          PriceButtonModel priceButton, 
                                                          Action<string> onClick)
        { 
            return new ProductItemModel {
                    ProductId = purchase.ProductId,
                    CountText = $"{purchase.GetRewards(packs).First().Count}",
                    IconPath = Util.IconPath.GetShopProduct(purchase.ProductId),
                    PriceButton = priceButton,
                    OnBuyClick = () => onClick.Invoke(purchase.ProductId),
            };
        }
        public static ProductItemModel CreateFromShop(ProductConfig productConfig, PackConfigCollection packs, 
                                                      PriceButtonModel priceButton,
                                                      Action<string> onClick)
        { 
            return new ProductItemModel {
                    ProductId = productConfig.ProductId,
                    CountText = $"{productConfig.GetRewards(packs).First().Count}",
                    IconPath = Util.IconPath.GetShopProduct(productConfig.ProductId),
                    PriceButton = priceButton,
                    OnBuyClick = () => onClick.Invoke(productConfig.ProductId),
            };
        }
    }
}