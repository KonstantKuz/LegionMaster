using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena.Model;
using UnityEngine;
using Zenject;

namespace LegionMaster.Location.GridArena
{
    public class ArenaGridProvider : IGridPositionProvider
    {
        [Inject]
        private LocationArena _arena;
        public Vector3 GetCellPos(CellId cellId)
        { 
            return _arena.CurrentGrid.GetCellPos(cellId);
        }
        public CellId GetCellByPos(Vector3 pos)
        {
            return _arena.CurrentGrid.GetCellByPos(pos);
        }
    }
}