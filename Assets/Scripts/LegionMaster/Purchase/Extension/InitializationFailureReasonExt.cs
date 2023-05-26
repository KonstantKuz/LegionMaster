using UnityEngine;
using UnityEngine.Purchasing;

namespace LegionMaster.Purchase.Extension
{
    public static class InitializationFailureReasonExt
    {
        public static void Log(this InitializationFailureReason error)
        {
            switch (error) {
                case InitializationFailureReason.AppNotKnown:
                    Debug.Log("Billing initialize FAIL. Code AppNotKnown");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    // Ask the user if billing is disabled in device settings.
                    Debug.Log("Billing initialize FAIL. Billing disabled!");
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    // Developer configuration error; check product metadata.
                    Debug.Log("Billing initialize FAIL. No products available for purchase!");
                    break;
                default:
                    Debug.LogError("Billing initialize FAIL. Unhandled code=" + error);
                    break;
            }
        }
    }
}