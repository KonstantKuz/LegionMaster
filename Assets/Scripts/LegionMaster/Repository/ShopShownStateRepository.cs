using LegionMaster.Shop.Data;

namespace LegionMaster.Repository
{
    public class ShopShownStateRepository : LocalPrefsSingleRepository<ShopShownState>
    {
        public ShopShownStateRepository() : base("shopShownState")
        {
        }
    }
}