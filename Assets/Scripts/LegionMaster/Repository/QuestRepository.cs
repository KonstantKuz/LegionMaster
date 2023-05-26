using LegionMaster.Quest.Model;

namespace LegionMaster.Repository
{
    public class QuestRepository: LocalPrefsSingleRepository<QuestCollection>
    {
        public QuestRepository() : base("quest")
        {
        }
    }
}