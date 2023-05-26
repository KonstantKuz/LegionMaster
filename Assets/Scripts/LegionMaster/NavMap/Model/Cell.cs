using JetBrains.Annotations;
using LegionMaster.Location.GridArena.Model;

namespace LegionMaster.NavMap.Model
{
    public class Cell
    {
        public CellId CellId { get; }
        public CellState State { get; private set; }
        
        [CanBeNull]
        public string UnitId { get; private set; }
        public Cell(CellId cellId, CellState state, [CanBeNull] string unitId = null)
        {
            CellId = cellId;
            State = state;
            UnitId = unitId;
        }

        public void Update(CellState state, [CanBeNull] string unitId = null)
        {
            State = state;
            UnitId = unitId;
        }
    }
}