using System.Collections.Generic;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Service
{
    public interface IRewardApplyService
    {
        void ApplyReward(RewardItem rewardItem);
        public void ApplyRewards(IEnumerable<RewardItem> items);
    }
}