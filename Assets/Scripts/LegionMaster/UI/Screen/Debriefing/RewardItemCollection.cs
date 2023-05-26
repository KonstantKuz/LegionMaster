using System.Collections.Generic;
using System.Linq;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class RewardItemCollection
    {
        private ICollection<RewardItemModel> _rewards;
        public RewardItemCollection(IEnumerable<RewardItemModel> rewards)
        {
            _rewards = rewards.ToList();
        }
        public ICollection<RewardItemModel> Rewards => _rewards;
        public IEnumerator<RewardItemModel> RewardsEnumerator => Rewards.GetEnumerator();
    }
}