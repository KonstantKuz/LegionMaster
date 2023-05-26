using LegionMaster.Quest.Model;

namespace LegionMaster.Repository
{
    public class QuestSectionRepository: LocalPrefsSingleRepository<QuestSectionCollection>
    {
        public QuestSectionRepository() : base("quest_section")
        {
        }
    }
}