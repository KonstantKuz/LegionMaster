using System.Runtime.Serialization;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class AdditionalBattleRewardConfig
    {
        [DataMember]
        public int BattleId;

        [DataMember]
        public string RewardId;

        [DataMember]
        public RewardType Type;

        [DataMember]
        public int Count;
    }
}