using System.Runtime.Serialization;
using JetBrains.Annotations;
using LegionMaster.BattlePass.Model;
using LegionMaster.Reward.Model;

namespace LegionMaster.BattlePass.Config
{
    [DataContract]
    public class BattlePassConfig
    {
        [DataMember]
        public int Level;
        [DataMember]
        public int ExpToNextLevel;
        [DataMember]
        public string BasicRewardId;
        [DataMember]
        public RewardType BasicRewardType;
        [DataMember]
        public int BasicRewardCount;
        [DataMember]
        public string PremiumRewardId;
        [DataMember]
        public RewardType PremiumRewardType;
        [DataMember]
        public int PremiumRewardCount;
        
        [CanBeNull]
        public RewardItem GetReward(BattlePassRewardType type)
        {
            return type == BattlePassRewardType.Basic ? GetBasicReward() : GetPremiumReward();
        }
        [CanBeNull]
        private RewardItem GetBasicReward()
        {
            return BasicRewardId == null ? null : new RewardItem(BasicRewardId, BasicRewardType, BasicRewardCount);
        }
        [CanBeNull]
        private RewardItem GetPremiumReward()
        {
            return PremiumRewardId == null ? null : new RewardItem(PremiumRewardId, PremiumRewardType, PremiumRewardCount);
        }
    }
}