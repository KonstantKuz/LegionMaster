using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.GridArena.Model;
using ModestTree;

namespace LegionMaster.NavMap.Model
{
    public class TargetSearchResult 
    {
        public Cell TargetCell { get; }
        public List<CellId> Path { get; }
        public CellId StartCell => Path.First();       
        public CellId FinishCell => Path.Last();
        public bool NoPath => Path == null || Path.IsEmpty();

        public bool CanAttackFromStartingPosition => TargetCell != null && NoPath;
        
        public TargetSearchResult(List<CellId> path, Cell targetCell)
        {
            Path = path;
            TargetCell = targetCell;
        }

    }
}