using EasyButtons;
using LegionMaster.Purchase.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.Purchase
{
    public class PurchaseTestSceneSetup : MonoBehaviour
    {
        [Inject] private InAppPurchaseService _inAppPurchaseService;
        
        [Button("Buy", Mode = ButtonMode.EnabledInPlayMode)]
        private void Buy(string productId)
        {
            _inAppPurchaseService.Buy(productId, OnPurchaseSuccess, OnPurchaseError);
        }

        private void OnPurchaseSuccess(string productId)
        {
            Debug.Log($"OnPurchaseSuccess: productId:= {productId}");
        }  
        private void OnPurchaseError(bool cancelled, string message)
        {
            if (cancelled) {
                Debug.Log($"Purchase canceled");
                return;
            }
            Debug.LogError($"OnPurchaseError: error:= {message}");
        }

    }
}