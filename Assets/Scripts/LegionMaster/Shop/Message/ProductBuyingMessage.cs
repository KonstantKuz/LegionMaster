
namespace LegionMaster.Shop.Message
{
    public struct ProductBuyingMessage
    {
        public string ProductId { get; }

        public ProductBuyingMessage(string productId)
        {
            ProductId = productId;
        }
    }
}