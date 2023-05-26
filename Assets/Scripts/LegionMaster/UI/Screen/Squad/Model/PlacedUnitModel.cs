using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.GridArena.Model;

namespace LegionMaster.UI.Screen.Squad.Model
{
    public class PlacedUnitModel
    {
        public string Id;
        public CellId CellId;
        public bool IsPlaced => CellId != CellId.InvalidCellId;

        public int Star;

        public bool CanMergeWith(string unitId, int starCount)
        {
            return unitId == Id && starCount == Star;
        }

        public static IEnumerable<PlacedUnitModel> GetMergeableUnits(IEnumerable<PlacedUnitModel> placedUnits, string unitId, int unitStar)
        {
            return placedUnits.Where(it => it.CanMergeWith(unitId, unitStar));
        }
    }
}