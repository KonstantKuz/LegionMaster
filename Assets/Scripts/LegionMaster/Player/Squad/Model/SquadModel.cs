using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.Session.Config;

namespace LegionMaster.Player.Squad.Model
{
    public class SquadModel
    {
        public List<UnitSpawnConfig> Units;

        public SquadModel()
        {
            Units = new List<UnitSpawnConfig>();
        }
        public UnitSpawnConfig GetUnit(string id) => Units.FirstOrDefault(it => it.UnitId == id);
    }
}