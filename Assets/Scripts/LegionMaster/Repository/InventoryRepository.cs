using LegionMaster.Player.Inventory.Model;

namespace LegionMaster.Repository
{
    public class InventoryRepository : LocalPrefsSingleRepository<Inventory>
    {
        protected InventoryRepository() : base("inventory")
        {
        }
    }
}