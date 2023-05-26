using System;
using LegionMaster.Purchase.Data;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.Util;
using UniRx;

namespace LegionMaster.UI.Screen.Description.Model
{
    public class PriceButtonModel
    {
        public decimal Price;
        public string PriceLabel;
        public bool Enabled;
        public IObservable<bool> CanBuy;
        public string CurrencyIconPath;
        public bool ShowIcon => CurrencyIconPath != null;
        
        public static PriceButtonModel FromProduct(ProductConfig product, ShopService shop)
        {
            return new PriceButtonModel() {
                    Price = product.CurrencyCount,
                    PriceLabel = product.CurrencyCount.ToString(),
                    Enabled = true,
                    CanBuy = shop.HasEnoughCurrencyAsObservable(product.ProductId),
                    CurrencyIconPath = IconPath.GetCurrency(product.Currency.ToString())
            };
        } 
        public static PriceButtonModel FromPurchase(BillingProductModel billingProductModel)
        {
            return new PriceButtonModel() {
                    Price = billingProductModel.LocalizedPrice,
                    PriceLabel = billingProductModel.LocalizedPriceString,
                    Enabled = true,
                    CanBuy = new BoolReactiveProperty(true),
                    CurrencyIconPath = null,
            };
        }
    }
}