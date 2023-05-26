using UnityEngine.Purchasing;

namespace LegionMaster.Purchase.Extension
{
    public static class ProductMetadataExtension
    {
        public static string AsText(this ProductMetadata metadata)
        {
            return $"Title={metadata.localizedTitle}, Description={metadata.localizedDescription}, "
                   + $"Price={metadata.localizedPriceString}, Currency={metadata.isoCurrencyCode}";
        }
    }
}