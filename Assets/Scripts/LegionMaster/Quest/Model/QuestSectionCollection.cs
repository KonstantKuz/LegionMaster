using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LegionMaster.Quest.Model
{
    [DataContract]
    public class QuestSectionCollection
    {
        [DataMember] 
        public List<QuestSectionRewardId> TakenRewards = new List<QuestSectionRewardId>();

        public bool IsSectionRewardTaken(QuestSectionRewardId rewardId)
        {
            return TakenRewards.Contains(rewardId);
        }
    }
}