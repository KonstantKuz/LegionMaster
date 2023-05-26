using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Extension;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Extension;
using LegionMaster.NavMap.Model;
using LegionMaster.NavMap.Service;
using UnityEngine;
using ModestTree;
using Zenject;

namespace LegionMaster.Units.Component.Ability
{
    public class BulldozingPathBuilder
    {
        private readonly NavMapService _navMapService;

        public BulldozingPathBuilder(NavMapService navMapService)
        {
            _navMapService = navMapService;
        }
        
        [CanBeNull]
        public IEnumerable<CellId> Build(CellId startCell, UnitType targetUnit)
        {
            IEnumerable<CellId> pathToMapEdge = null;
            var targetCellId = CellId.InvalidCellId;
            var minDistance = Mathf.Infinity;
            foreach (var direction in EnumExt.Values<Direction>())
            {
                var currentLine = _navMapService.NavMap.GetPathToEdge(startCell, direction).ToArray();
                var cellWithTarget = GetFirstCellWithTarget(targetUnit, currentLine);
                if (cellWithTarget == CellId.InvalidCellId || HasObstacleOnPath(currentLine, cellWithTarget))
                {
                    continue;
                }
                var distanceToOrigin = cellWithTarget.DistanceTo(startCell);
                if (distanceToOrigin > minDistance)
                {
                    continue;
                }
                minDistance = distanceToOrigin;
                targetCellId = cellWithTarget;
                pathToMapEdge = currentLine;
            }
            return pathToMapEdge == null ? null : BuildPathToFirstObstacle(startCell, targetCellId, pathToMapEdge);
        }

        private CellId GetFirstCellWithTarget(UnitType targetUnit, IEnumerable<CellId> pathToEdge)
        {
            foreach (var cellId in pathToEdge)
            {
                if (GetCell(cellId).State == targetUnit.ToCellState())
                {
                    return cellId;
                }
            }

            return CellId.InvalidCellId;
        }

        private bool HasObstacleOnPath(CellId[] currentLine, CellId cellWithTarget)
        {
            for (int cellIndex = 1; cellIndex < currentLine.IndexOf(cellWithTarget); cellIndex++)
            {
                if (GetCell(currentLine[cellIndex]).State != CellState.Empty)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<CellId> BuildPathToFirstObstacle(CellId startCell, CellId targetCellId, IEnumerable<CellId> pathToMapEdge)
        {
            return pathToMapEdge.TakeWhile(it => GetCell(it).State == CellState.Empty || 
                                               GetCell(it).UnitId == GetCell(targetCellId).UnitId || 
                                               GetCell(it).UnitId == GetCell(startCell).UnitId);
        }

        private Cell GetCell(CellId cellId) => _navMapService.GetCell(cellId);
    }
}