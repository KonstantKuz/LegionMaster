using System;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Model;
using LegionMaster.Units.Component;
using UnityEngine;
using Zenject;

namespace LegionMaster.NavMap.Service
{
    public class NavMapService
    {
        private NavMap _navMap;
        private bool _debugPrintMapEnabled = false;

        [Inject]
        private LocationArena _locationArena;

        public NavMap NavMap => _navMap;

        public void InitMap()
        {
            CreateMap(CreateEmptyCells(_locationArena.CurrentGrid.SizeY, _locationArena.CurrentGrid.SizeX));
        }

        private static Cell[,] CreateEmptyCells(int sizeY, int sizeX)
        {
            var cells = new Cell[sizeY, sizeX];
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    cells[y, x] = new Cell(new CellId(y, x), CellState.Empty);
                }
            }

            return cells;
        }

        public void CreateMap(Cell[,] map)
        {
            _navMap = new NavMap(map);
        }

        [CanBeNull]
        public Cell GetNearestEmptyCell(CellId toCellId)
        {
            var maxSearchRadius = Mathf.Max(_navMap.Map.GetLength(0), _navMap.Map.GetLength(1));
            var emptyCell = _navMap.GetNeighbours(toCellId, maxSearchRadius)
                .Select(GetCell).FirstOrDefault(it => IsEmpty(it.CellId));
           return emptyCell;
        }
        
        public void MoveUnit(CellId from, CellId to, UnitType unitType, string unitId)
        {
            var fromCell = GetCell(from);
            if (fromCell.State != unitType.ToCellState() || fromCell.UnitId != unitId) {
                throw new Exception($"FromCell invalid, state={fromCell.State}, unitId={fromCell.UnitId}");
            }
            var toCell = GetCell(to);
            if (toCell.State != CellState.Empty) {
                throw new Exception($"ToCell is not Empty, state={toCell.State}, unitId={toCell.State}");
            }
            _navMap.UpdateCell(from, CellState.Empty); 
            _navMap.UpdateCell(to, unitType.ToCellState(), unitId);
            PrintMap();
        }

        public Cell GetCell(CellId cell) => _navMap.GetCell(cell);
        public bool IsEmpty(CellId cellId) => GetCell(cellId).State == CellState.Empty;

        public TargetSearchResult FindPath(CellId startCell, UnitType targetUnit, int attackRange)
        {
            return _navMap.FindPath(startCell, targetUnit.ToCellState(), attackRange);
        }

        public void UpdateUnitCell(CellId cellId, UnitType unitType, string targetId)
        {
            var oldCellId = _navMap.FindCellByTargetId(targetId);
            if (oldCellId == cellId) return;
            if (oldCellId != CellId.InvalidCellId)
            {
                _navMap.UpdateCell(oldCellId, CellState.Empty);    
            }

            var cell = _navMap.GetCell(cellId);
            if (cell.State == CellState.Empty)
            {
                _navMap.UpdateCell(cellId, unitType.ToCellState(), targetId);
            }

            PrintMap();
        }

        public void RemoveUnitFromCell(CellId cellId, string targetId)
        {
            var cell = _navMap.GetCell(cellId);
            if (cell.UnitId != targetId && !IsReservedByShieldBearerAbility(cell)) { // TODO: пофиксить проблему смерти юнита во время абилки щитоносца
                throw new Exception($"RemoveUnitFromCell: expected unit in cell {targetId}, actual unit in cell {cell.UnitId}");
            }
            _navMap.UpdateCell(cellId, CellState.Empty);
            PrintMap();
        }
        
        private bool IsReservedByShieldBearerAbility(Cell cell)
        {
            return cell.State == CellState.Reserved && cell.UnitId != null && cell.UnitId.Contains("UnitShieldBearer");
        }

        private void PrintMap()
        {
            if (!_debugPrintMapEnabled) return;
            
            var msg = "\n";
            msg += new string('-', _navMap.Map.GetLength(1) + 2) + "\n";
            for (var y = _navMap.Map.GetLength(0) - 1; y >= 0; y--)
            {
                var line = "";
                for (var x = 0; x < _navMap.Map.GetLength(1); x++)
                {
                    line += _navMap.GetState(new CellId(y, x)) switch {
                        CellState.Empty => "  ",
                        CellState.BusyPlayer => "p",
                        CellState.BusyEnemy => "e",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
                msg += $"|{line}|\n";
            }
            msg += new string('-', _navMap.Map.GetLength(1) + 2) + "\n";
            Debug.Log(msg);
        }
    }
}