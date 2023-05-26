using System.Linq;
using LegionMaster.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Data;
using LegionMaster.Shop.Message;
using SuperMaxim.Messaging;

namespace LegionMaster.Purchase.Service
{
    public class InAppPurchaseService
    {
        private readonly StringKeyedConfigCollection<PurchaseConfig> _purchases;
        private readonly BillingService _billingService;
        private readonly IMessenger _messenger;
        public InAppPurchaseService(StringKeyedConfigCollection<PurchaseConfig> purchases, IMessenger messenger)
        {
            _purchases = purchases;
            _messenger = messenger;
            _billingService = CreateBilling();
        }
        private BillingService CreateBilling()
        {
            var products = _purchases.Values
                                     .ToDictionary(it => it.StoreProductId, it => it.ProductType);
            return new BillingService().Init(new LocalReceiptValidator(), products);
        }
        public void Buy(string productId, PurchaseSuccess successCallback, PurchaseError errorCallback)
        {
            var purchase = _purchases.Get(productId);
            _billingService.Purchase(purchase.StoreProductId, storeId => {
                successCallback?.Invoke(productId);
                _messenger.Publish(new ProductBuyingMessage(productId));
            }, errorCallback);
        }
        public BillingProductModel GetProduct(string productId)
        {
            if (!_billingService.Initialized) {
                return null;
            }
            var purchase = _purchases.Get(productId);
            return _billingService.AvailableProducts.FirstOrDefault(it => it.StoreProductId.Equals(purchase.StoreProductId));
        }
        public BillingService BillingService => _billingService;
        public bool IsBillingInitialized => _billingService is { Initialized: true };
    }
}