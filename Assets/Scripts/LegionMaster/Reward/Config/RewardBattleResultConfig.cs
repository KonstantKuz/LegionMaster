using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Location.Session.Model;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class RewardBattleResultConfig
    {
        [DataMember(Name = "BattleResult")]
        public BattleResult BattleResult;
        
        [DataMember(Name = "Rewards")]
        public IReadOnlyList<RewardBattleConfig> Rewards;
    }
}