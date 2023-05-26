using JetBrains.Annotations;
using LegionMaster.Reward.Model;

namespace LegionMaster.Repository
{
    [PublicAPI]
    public class RewardSiteStateRepository : LocalPrefsSingleRepository<RewardSiteStateCollection>
    {
        protected RewardSiteStateRepository() : base("rewardSiteState")
        {
        }
    }
}