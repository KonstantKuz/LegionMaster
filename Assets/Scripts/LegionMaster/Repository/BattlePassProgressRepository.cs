using LegionMaster.BattlePass.Model;

namespace LegionMaster.Repository
{
    public class BattlePassProgressRepository : LocalPrefsSingleRepository<BattlePassProgress>
    {
        protected BattlePassProgressRepository() : base("battlePassProgress")
        {
        }
    }
}