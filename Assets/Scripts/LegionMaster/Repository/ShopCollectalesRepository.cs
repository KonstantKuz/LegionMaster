using LegionMaster.Shop.Data;

namespace LegionMaster.Repository
{
    public class ShopCollectablesRepository : LocalPrefsSingleRepository<ProductCollectables>
    {
        protected ShopCollectablesRepository() : base("shopCollectables")
        {
        }
    }
}