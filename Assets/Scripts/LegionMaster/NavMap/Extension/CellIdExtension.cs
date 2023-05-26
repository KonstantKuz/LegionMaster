using LegionMaster.Location.GridArena.Model;
using UnityEngine;

namespace LegionMaster.NavMap.Extension
{
    public static class CellIdExtension
    {
        public static float DistanceTo(this CellId origin, CellId target)
        {
            var originPoint = new Vector2(origin.X, origin.Y);
            var targetPoint = new Vector2(target.X, target.Y);
            return Vector2.Distance(originPoint, targetPoint);
        }
    }
}