using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Quest.Model;
using LegionMaster.Reward.Config;

namespace LegionMaster.Quest.Config
{
    [DataContract]
    public class QuestConfig : ICollectionItem<string>
    {
        [DataMember(Name = "Id")]
        private string _id;
        [DataMember]
        public QuestSectionType Section;
        [DataMember]
        public string Condition;
        [DataMember]
        public int ConditionCount;
        [DataMember]
        public int Points;
        [DataMember]
        public RewardItemConfig Reward;
        
        public string Id
        {
            get => _id;
            set => _id = value;
        }
    
    }
}