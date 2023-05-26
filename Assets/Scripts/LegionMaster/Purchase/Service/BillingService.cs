using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Purchase.Data;
using LegionMaster.Purchase.Extension;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using UnityEngine.Purchasing;

namespace LegionMaster.Purchase.Service
{
    public delegate void PurchaseSuccess(string productId);

    public delegate void PurchaseError(bool cancelled, string message);

    public class BillingService : IStoreListener
    {
        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        private FakeStoreUIMode _fakeMode;

        private PurchaseSuccess _successCallback;
        private PurchaseError _errorCallback;

        private bool _fake;
        private bool _purchaseInProgress;

        private IBillingReceiptValidator _receiptValidator;
        private IEnumerable<BillingProductModel> _availableProducts;

        public event Action<bool> OnBillingInitialized;
        public InitializationFailureReason InitializationError { get; private set; }
        public AppStore CurrentStore { get; private set; }

        public BillingService Init(IBillingReceiptValidator receiptValidator,
                                   Dictionary<string, ProductType> products,
                                   bool fake = false,
                                   FakeStoreUIMode fakeStoreUIMode = FakeStoreUIMode.StandardUser)
        {
#if UNITY_STANDALONE
            fake = true;
#endif
            _fake = fake;
            _fakeMode = fakeStoreUIMode;
            _receiptValidator = receiptValidator;
            
            var module = StandardPurchasingModule.Instance(AppStore);
            SetFakeStoreInModule(module);
            CurrentStore = module.appStore;

            var builder = ConfigurationBuilder.Instance(module);
            
            RegisterProducts(builder, products);
            UnityPurchasing.Initialize(this, builder);
            return this;
        }

        public void Purchase(string productId, PurchaseSuccess successCallback, PurchaseError errorCallback)
        {
            Debug.Log("Purchase productId=" + productId);

            if (!Initialized) {
                Debug.LogWarning("Buy productId=" + productId + " FAIL. Not initialized.");
                errorCallback.Invoke(false, "BuyProductId FAIL. Not initialized.");
                return;
            }
            if (_purchaseInProgress) {
                Debug.LogWarning("Buy productId=" + productId + ". Please wait, purchase in progress");
                errorCallback.Invoke(false, "Please wait, purchase in progress");
                return;
            }
            var product = _storeController.products.WithID(productId);

            if (product == null) {
                Debug.LogError("Buy productId=" + productId + ". No product found.");
                errorCallback.Invoke(false, "No product found in store");
                return;
            }

            if (!product.availableToPurchase) {
                Debug.LogError("Buy productId=" + productId + ". Not purchasing product, either is not available for purchase");
                errorCallback.Invoke(false, "Product not available for buy.");
                return;
            }

            _successCallback = successCallback;
            _errorCallback = errorCallback;

            _purchaseInProgress = true;
            _storeController.InitiatePurchase(product);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("Process purchase: " + e.purchasedProduct.definition.id + " with receipt " + e.purchasedProduct.receipt);
            var product = e.purchasedProduct;
            _receiptValidator.Validate(product, CurrentStore, OnValidateReceiptSuccess, OnValidateReceiptError);
            return PurchaseProcessingResult.Pending;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("Billing initialized");
            _storeController = controller;
            _storeExtensionProvider = extensions;
            OnBillingInitialized?.Invoke(true);
        }

        public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
        {
            if (IsStandardFakeDialog(r)) {
                // Bug, FakeStoreUIMode.StandardUser on Cancel button returns PurchasingUnavailable instead of UserCancelled
                r = PurchaseFailureReason.UserCancelled;
            }
            if (r == PurchaseFailureReason.UserCancelled) {
                Debug.Log($"Purchase cancelled by productId: {item.definition.id}");
            } else {
                Debug.LogWarning($"Purchase failed: id={item.definition.id} product=({item.metadata.AsText()}, PurchaseFailureReason= {r.ToString()})");
            }
            _errorCallback?.Invoke(r == PurchaseFailureReason.UserCancelled, "Purchase failed: " + r);
            CompletePurchase();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogWarning("Billing failed to initialize!");
            error.Log();
            OnBillingInitialized?.Invoke(false);
            InitializationError = error;
        }

        private void SetFakeStoreInModule(StandardPurchasingModule module)
        {
            module.useFakeStoreUIMode = _fakeMode;
            module.useFakeStoreAlways = _fake;
        }

        private bool IsStandardFakeDialog(PurchaseFailureReason r)
        {
            return _fake && _fakeMode == FakeStoreUIMode.StandardUser && r == PurchaseFailureReason.PurchasingUnavailable;
        }

        private void OnValidateReceiptSuccess(string productId)
        {
            Debug.Log("Validate receipt succes for product=" + productId);

            var product = _storeController.products.WithID(productId);
            _storeController.ConfirmPendingPurchase(product);

            _successCallback?.Invoke(productId);
            CompletePurchase();
        }

        private void OnValidateReceiptError(string message)
        {
            Debug.LogWarning("Validate receipt error=" + message);
            _errorCallback?.Invoke(false, message);
            CompletePurchase();
        }

        private void CompletePurchase()
        {
            _purchaseInProgress = false;
            _successCallback = null;
            _errorCallback = null;
        }

        private void RegisterProducts(ConfigurationBuilder builder, Dictionary<string, ProductType> products)
        {
            products.ForEach(it => { builder.AddProduct(it.Key, it.Value); });
        }

        private AppStore AppStore
        {
            get
            {
                if (_fake) {
                    return AppStore.fake;
                }
                return Application.platform switch {
                        RuntimePlatform.Android => AppStore.GooglePlay,
                        RuntimePlatform.IPhonePlayer => AppStore.AppleAppStore,
                        _ => Application.platform == RuntimePlatform.OSXPlayer ? AppStore.MacAppStore : AppStore.NotSpecified
                };
            }
        }

        private IEnumerable<BillingProductModel> BuildAvailableProducts()
        {
            return _storeController.products.all.Where(item => item.availableToPurchase)
                                   .Select(item => new BillingProductModel {
                                           StoreProductId = item.definition.id,
                                           LocalizedTitle = item.metadata.localizedTitle,
                                           LocalizedDescription = item.metadata.localizedDescription,
                                           ISOCurrencyCode = item.metadata.isoCurrencyCode,
                                           LocalizedPrice = item.metadata.localizedPrice,
                                           LocalizedPriceString = item.metadata.localizedPriceString
                                   });
        }

        public IEnumerable<BillingProductModel> AvailableProducts => _availableProducts ??= BuildAvailableProducts();
        public bool Initialized => _storeController != null && _storeExtensionProvider != null;
    }
}