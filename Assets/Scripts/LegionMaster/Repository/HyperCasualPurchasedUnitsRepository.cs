using LegionMaster.HyperCasual.Store.Data;

namespace LegionMaster.Repository
{
    public class HyperCasualPurchasedUnitsRepository : LocalPrefsSingleRepository<PurchasedUnits>
    {
        public HyperCasualPurchasedUnitsRepository() : base("hyperCasualPurchasedUnits_v1")
        {
        }
    }
}