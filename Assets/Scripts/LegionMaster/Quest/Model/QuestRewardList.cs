using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Reward.Config;

namespace LegionMaster.Quest.Model
{
    [DataContract]
    public class QuestRewardList: List<RewardItemConfig>
    {
    }
}