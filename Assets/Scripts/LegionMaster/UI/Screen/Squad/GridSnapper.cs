using System.Linq;
using LegionMaster.Location.GridArena;
using LegionMaster.Units.Component;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad
{
    public class GridSnapper
    {
        private readonly ArenaGrid _grid;
        private readonly float _snapDistance;

        public GridSnapper(ArenaGrid grid, float snapDistance)
        {
            _grid = grid;
            _snapDistance = snapDistance;
        }
        public GridCell Snap(Vector3 pos, UnitType unitType)
        {
            var closestCell = _grid.GetCells(unitType).OrderBy(cell => Vector3.Distance(cell.transform.position, pos)).First();
            return Vector3.Distance(pos, closestCell.transform.position) <= _snapDistance ? closestCell : null;
        }
    }
}