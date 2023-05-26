using LegionMaster.Cheats;

namespace LegionMaster.Repository
{
    public class CheatRepository : LocalPrefsSingleRepository<CheatSettings>
    {
        public CheatRepository() : base("CheatSettings")
        {
        }
    }
}