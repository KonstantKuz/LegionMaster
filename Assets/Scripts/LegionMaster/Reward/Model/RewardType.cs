using LegionMaster.Player.Inventory.Model;
using LegionMaster.Reward.Config;
using LegionMaster.Util;

namespace LegionMaster.Reward.Model
{
    public enum RewardType
    {
        None,
        Shards,
        LootBox,
        Currency,
        Exp,
        BattlePassExp, 
        BattlePassPremium,
        
    }
    public static class RewardTypeExtension
    {
        public static bool ShouldPlayDropVfx(this RewardType rewardType)
        {
            return rewardType == RewardType.Currency || rewardType == RewardType.Exp || rewardType == RewardType.BattlePassExp;
        }
        public static string GetIconPath(this RewardConfig rewardConfig)
        {
            return GetIconPath(rewardConfig.Type, rewardConfig.Id);
        }
        public static string GetIconPath(this RewardItem rewardItem)
        {
            return GetIconPath(rewardItem.RewardType, rewardItem.RewardId);
        }
        public static string GetBackgroundPath(this RewardItem rewardItem)
        {
            return IconPath.GetRewardBackground(rewardItem.RewardId == Currency.Soft.ToString() ? Currency.Soft.ToString() : IconPath.REGULAR_REWARD_BACKGROUND_ID);
        }
        public static string GetIconPath(RewardType rewardType, string rewardId)
        {
            return rewardType switch {
                    RewardType.Shards => IconPath.GetUnitVertical(rewardId),
                    RewardType.LootBox => IconPath.GetReward(RewardType.LootBox.ToString()),
                    _ => IconPath.GetReward(rewardId)
            };
        }
    }
}