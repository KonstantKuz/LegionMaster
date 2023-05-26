using UnityEngine.Purchasing;

namespace LegionMaster.Purchase.Service
{
    public delegate void SuccessCallback(string productId);
    
    public delegate void ErrorCallback(string message);

    public interface IBillingReceiptValidator
    {
        void Validate(Product product, AppStore storeType, SuccessCallback onSuccess, ErrorCallback onError);
    }
}