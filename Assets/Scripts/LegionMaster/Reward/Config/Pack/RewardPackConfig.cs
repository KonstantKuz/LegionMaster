using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LegionMaster.Reward.Config.Pack
{
    [DataContract]
    public class RewardPackConfig
    {
        [DataMember]
        public string PackId;
        [DataMember]
        public IEnumerable<RewardItemConfig> Rewards;
    }
}