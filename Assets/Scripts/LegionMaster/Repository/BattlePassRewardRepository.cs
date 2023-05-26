using LegionMaster.BattlePass.Model;

namespace LegionMaster.Repository
{
    public class BattlePassRewardRepository : LocalPrefsSingleRepository<BattlePassRewardCollection>
    {
        protected BattlePassRewardRepository() : base("battlePassReward")
        {
        }
    }
}