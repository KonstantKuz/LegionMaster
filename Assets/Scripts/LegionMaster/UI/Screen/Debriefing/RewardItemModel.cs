using LegionMaster.Reward.Model;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class RewardItemModel
    {
        public string RewardLabel; 
        public string RewardCount;
        public string IconPath;
        public RewardItem RewardItem;
        
        public string BackgroundPath;
        public bool IsUnitShards;

        public static RewardItemModel CreateForDebriefing(string countPrefix, RewardItem rewardItem)
        {
            var isShards = rewardItem.RewardType == RewardType.Shards;
            return new RewardItemModel {
                    RewardLabel = isShards ? $"{rewardItem.RewardId}Shard" : rewardItem.RewardId,
                    RewardCount = countPrefix + rewardItem.Count,
                    IconPath = isShards ? Util.IconPath.GetUnitVertical(rewardItem.RewardId) : Util.IconPath.GetCurrency(rewardItem.RewardId),
                    RewardItem = rewardItem,
                    BackgroundPath = isShards ? Util.IconPath.GetRewardBackground(Util.IconPath.REGULAR_REWARD_BACKGROUND_ID) : null,
                    IsUnitShards = isShards
            };
        } 
        public static RewardItemModel Create(string rewardLabel, string countPrefix, RewardItem rewardItem)
        {
            return new RewardItemModel {
                    RewardLabel = rewardLabel,
                    RewardCount = countPrefix + rewardItem.Count,
                    IconPath = rewardItem.GetIconPath(),
                    RewardItem = rewardItem,
                    BackgroundPath = rewardItem.GetBackgroundPath(),
            };
        }
    }
}