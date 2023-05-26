using LegionMaster.Player.Squad.Model;

namespace LegionMaster.Repository
{
    public class PlayerHyperCasualSquadRepository : LocalPrefsSingleRepository<SquadModel>
    {
        protected PlayerHyperCasualSquadRepository() : base("hyperCasualSquad_v1")
        {
        }
    }
}