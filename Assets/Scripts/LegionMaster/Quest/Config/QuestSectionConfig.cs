using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Quest.Model;
using LegionMaster.Reward.Config;

namespace LegionMaster.Quest.Config
{
    [DataContract]
    public class QuestSectionRewardConfig : ICollectionItem<QuestSectionRewardId>
    {
        [DataMember]
        public QuestSectionType Section;
        [DataMember]
        public int RequiredPoints;
        [DataMember]
        public RewardItemConfig Reward;

        public QuestSectionRewardId Id => new QuestSectionRewardId
        {
                Type = Section,
                Points = RequiredPoints
        };
    }
}