using LegionMaster.Location.GridArena.Model;
using UnityEngine;

namespace LegionMaster.Location.GridArena
{
    public interface IGridPositionProvider
    {
        Vector3 GetCellPos(CellId cellId);
        CellId GetCellByPos(Vector3 pos);
    }
}