using System.Runtime.Serialization;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class RewardItemConfig
    {
        [DataMember(Name = "RewardType")]
        public RewardType Type;
        [DataMember(Name = "RewardId")]
        public string Id;
        [DataMember(Name = "RewardCount")]
        public int Count;

        public RewardItem ToRewardItem()
        {
            return new RewardItem(Id, Type, Count);
        }
    }
}