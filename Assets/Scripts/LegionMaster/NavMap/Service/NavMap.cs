using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Extension;
using LegionMaster.NavMap.Model;
using UnityEngine;
using LegionMaster.Units.Component.Target;

namespace LegionMaster.NavMap.Service
{
    public enum Direction
    {
        Forward,
        Back,
        Right,
        Left,
    }
    
    public class NavMap
    {
        private readonly Cell[,] _map;

        public Cell[,] Map => _map;
        public NavMap(Cell[,] map)
        {
            _map = map;
        }

        public void UpdateCell(CellId cellId, CellState state, [CanBeNull] string unitId = null) => _map[cellId.Y, cellId.X].Update(state, unitId);
        public CellState GetState(CellId cellId) => _map[cellId.Y, cellId.X].State;
        public Cell GetCell(CellId cellId) => _map[cellId.Y, cellId.X];
        
        public TargetSearchResult FindPath(CellId startCell, CellState targetState, int attackRange)
        {
            CellId finishPathCell = CellId.InvalidCellId;
            CellId targetCell = CellId.InvalidCellId;
            
            var predecessors = new Dictionary<CellId, CellId>();
            var visitedCells = new HashSet<CellId>();
            var queue = new Queue<CellId>();
            queue.Enqueue(startCell);
            
            while (queue.Count != 0) {
                var currentCell = queue.Dequeue();
                visitedCells.Add(currentCell);

                if (IsTargetInAttackRange(currentCell, targetState, attackRange, out targetCell)) {
                    finishPathCell = currentCell;
                    break;
                }
                foreach (var neighbor in GetNeighbours(currentCell)) {
                    if (visitedCells.Contains(neighbor) || CanNotBeVisited(neighbor)) {
                        continue;
                    }
                    predecessors[neighbor] = currentCell;
                    queue.Enqueue(neighbor);
                }
            }
    
            return BuildPath(startCell, finishPathCell, predecessors, targetCell);
        }

        public IEnumerable<CellId> GetPathToEdge(CellId fromCell, Direction direction)
        {
            var currentCell = fromCell;
            while (!IsOutsideMap(currentCell))
            {
                yield return currentCell;
                currentCell = GetNextCellByDirection(currentCell, direction);
            }
        }

        public IEnumerable<ITarget> GetEnemiesInRange(CellId cellId, CellState searchState, int maxRadius, TargetListProvider targetListProvider)
        {
            return GetNeighbours(cellId, maxRadius).Select(GetCell)
                                                   .Where(it => it.UnitId != null && it.State == searchState)
                                                   .Select(it => targetListProvider.FindById(it.UnitId));
        }

        public IEnumerable<CellId> GetNeighbours(CellId currentCell, int maxRadius)
        {
            for (int radius = 0; radius <= maxRadius; radius++)
            {
                var squareEnumerator = new SquareEnumerator(currentCell, radius);
                foreach (var cell in squareEnumerator)
                {
                    if (IsOutsideMap(cell))
                    {
                        continue;
                    }
                    
                    yield return cell;
                }
            }
        }

        private IEnumerable<CellId> GetNeighbours(CellId currentCell)
        {
            var neighbours = new CellId[] {
                    new CellId(currentCell.Y + 1, currentCell.X),
                    new CellId(currentCell.Y - 1, currentCell.X),
                    new CellId(currentCell.Y, currentCell.X + 1),
                    new CellId(currentCell.Y, currentCell.X - 1),
            };
            for (int i = 0; i < 4; i++) {
                if (!IsOutsideMap(neighbours[i])) {
                    yield return neighbours[i];
                }
            }
        }

        private CellId GetNextCellByDirection(CellId fromCell, Direction direction)
        {
            return direction switch
            {
                Direction.Forward => new CellId(fromCell.Y + 1, fromCell.X),
                Direction.Back => new CellId(fromCell.Y - 1, fromCell.X),
                Direction.Right => new CellId(fromCell.Y, fromCell.X + 1),
                Direction.Left => new CellId(fromCell.Y, fromCell.X - 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
            };
        }

        private bool IsTargetInAttackRange(CellId currentCell, CellState targetState, int attackRange, out CellId targetCell)
        {
            for (int radius = 0; radius <= attackRange; radius++) {
                var squareEnumerator = new SquareEnumerator(currentCell, radius);
                foreach (var searchedCell in squareEnumerator) {
                    if (IsOutsideMap(searchedCell)) {
                        continue;
                    }
                    if (GetState(searchedCell) != targetState) {
                        continue;
                    }
                    targetCell = searchedCell;
                    return true;
                }
            }
            targetCell = CellId.InvalidCellId;
            return false;
        }
        private bool IsOutsideMap(CellId cellId)
        {
            return cellId.X < 0 || cellId.X >= _map.GetLength(1) || cellId.Y < 0 || cellId.Y >= _map.GetLength(0);
        }

        private bool CanNotBeVisited(CellId cellId)
        {
            return GetState(cellId) != CellState.Empty;
        }

        private TargetSearchResult BuildPath(CellId startCell, CellId finishPathCell, Dictionary<CellId, CellId> predecessors, CellId targetCell)
        {
            if (finishPathCell == CellId.InvalidCellId) {
                return new TargetSearchResult(null, null);
            }
            if (startCell == finishPathCell) {
                return new TargetSearchResult(null, GetCell(targetCell));
            }

            var path = BuildPath(startCell, finishPathCell, predecessors);
            return new TargetSearchResult(path.ToList(), GetCell(targetCell));
        }

        private IEnumerable<CellId> BuildPath(CellId startCell, CellId finishPathCell, Dictionary<CellId, CellId> predecessors)
        {
            var path = new List<CellId> {finishPathCell};
            var lastCell = finishPathCell;
            while (predecessors[lastCell] != startCell) {
                lastCell = predecessors[lastCell];
                path.Add(lastCell);
            }
            path.Reverse();
            return path;
        }
        
        public CellId FindCellByTargetId(string targetId)
        {
            foreach (var cell in _map)
            {
                if (cell.UnitId == targetId) return cell.CellId;
            }
            return CellId.InvalidCellId;
        }
    }
}