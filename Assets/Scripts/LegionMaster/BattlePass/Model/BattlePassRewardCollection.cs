using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LegionMaster.BattlePass.Model
{
    [DataContract]
    public class BattlePassRewardCollection
    {
        [DataMember]
        public List<BattlePassRewardId> TakenRewards = new List<BattlePassRewardId>();
        
        public bool IsRewardTaken(BattlePassRewardId rewardId)
        {
            return TakenRewards.Contains(rewardId);
        }

        public void Add(BattlePassRewardId rewardId)
        {
            if (IsRewardTaken(rewardId)) { 
                throw new Exception($"RewardItem for {rewardId.ToString()} already taken");
            }
            TakenRewards.Add(rewardId);
        }
    }
}