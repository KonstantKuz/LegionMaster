namespace LegionMaster.Purchase.Data
{
    public class BillingProductModel
    {
        public string StoreProductId { get; set; }

        public string LocalizedTitle { get; set; }

        public string LocalizedDescription { get; set; }

        public string ISOCurrencyCode { get; set; }

        public decimal LocalizedPrice { get; set; }
        public string LocalizedPriceString { get; set; }
        
    }
}