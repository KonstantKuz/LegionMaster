using LegionMaster.Tutorial.Model;

namespace LegionMaster.Repository
{
    class TutorialRepository: LocalPrefsSingleRepository<TutorialState>
    {
        protected TutorialRepository() : base("tutorial")
        {
        }
    }
}