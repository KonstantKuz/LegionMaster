using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
namespace LegionMaster.Purchase.Service
{
    public class LocalReceiptValidator : IBillingReceiptValidator
    {
        public void Validate(Product product, AppStore currentAppStore, SuccessCallback onSuccess, ErrorCallback onError)
        {
            if (currentAppStore == AppStore.fake) {
                onSuccess?.Invoke(product.definition.id);
                return;
            }
            if (!IsCurrentStoreSupported(currentAppStore)) {
                InvokeOnError($"Unsupported stor: {currentAppStore}", onError);
                return;
            }
            ValidateReceipt(product, currentAppStore, onSuccess, onError);
        }

        private void ValidateReceipt(Product product, AppStore currentAppStore, SuccessCallback onSuccess, ErrorCallback onError)
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try {
                var receipts = validator.Validate(product.receipt);
                LogReceipts(receipts);
                onSuccess?.Invoke(product.definition.id);
            } catch (IAPSecurityException reason) {
                InvokeOnError($"Invalid receipt: {reason}", onError);
            }
#endif
            InvokeOnError($"Unsupported stor: {currentAppStore}", onError);
        }

        private void InvokeOnError(string message, ErrorCallback onError)
        {
            Debug.LogError(message);
            onError?.Invoke(message);
        }

        private static bool IsCurrentStoreSupported(AppStore currentAppStore) =>
                currentAppStore == AppStore.GooglePlay || currentAppStore == AppStore.AppleAppStore || currentAppStore == AppStore.MacAppStore;

        private static void LogReceipts(IEnumerable<IPurchaseReceipt> receipts)
        {
            Debug.Log("Receipt is valid. Contents:");
            foreach (var receipt in receipts) {
                LogReceipt(receipt);
            }
        }

        private static void LogReceipt(IPurchaseReceipt receipt)
        {
            Debug.Log($"Product ID: {receipt.productID}\n" 
                      + $"Purchase Date: {receipt.purchaseDate}\n" 
                      + $"Transaction ID: {receipt.transactionID}");

            if (receipt is GooglePlayReceipt googleReceipt) {
                Debug.Log($"Purchase State: {googleReceipt.purchaseState}\n" 
                          + $"Purchase Token: {googleReceipt.purchaseToken}");
            }

            if (receipt is AppleInAppPurchaseReceipt appleReceipt) {
                Debug.Log($"Original Transaction ID: {appleReceipt.originalTransactionIdentifier}\n"
                          + $"Subscription Expiration Date: {appleReceipt.subscriptionExpirationDate}\n"
                          + $"Cancellation Date: {appleReceipt.cancellationDate}\n" + $"Quantity: {appleReceipt.quantity}");
            }
        }
    }
}