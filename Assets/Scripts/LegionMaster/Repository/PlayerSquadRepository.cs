using LegionMaster.Player.Squad.Model;

namespace LegionMaster.Repository
{
    public class PlayerSquadRepository : LocalPrefsSingleRepository<SquadModel>
    {
        protected PlayerSquadRepository() : base("squad_v0.6.0")
        {
        }
    }
}