using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Location.GridArena.Config;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Units.Component;
using SuperMaxim.Core.Extensions;
using UnityEngine;

namespace LegionMaster.Location.GridArena
{
    public class ArenaGrid : MonoBehaviour, IGridPositionProvider
    {
        [SerializeField] private ArenaGridConfig _config; 
        
        private GridCell[] _allCells;
        private Dictionary<UnitType, List<GridCell>> _placementAreaCells;

        public ArenaGridConfig Config => _config;
        public float CellSize => _config.CellStep;

        public void Reset()
        {
            AllCells.ForEach(it => it.Reset());
        }

        public void HighlightCells(UnitType unitType, CellHighlight highlight)
        {
            GetCells(unitType).ForEach(it => it.Highlight(highlight));
        }

        public GridCell GetCell(CellId id)
        {
            return AllCells.First(g => g.Id == id);
        }
        public List<GridCell> GetCells(UnitType unitType) => PlacementAreaCells[unitType];
        public GridCell RandomFreeCell(UnitType unitType)
        {
            var freeCells = GetCells(unitType).Where(it => !it.IsFull).ToList();
            return freeCells.Count > 0 ? freeCells.Random() : null;
        }
        private Dictionary<UnitType, List<GridCell>> GetPlacementAreaCells()
        { 
            return Enum.GetValues(typeof(UnitType))
                       .OfType<UnitType>()
                       .Select(type => (type, GetPlacementAreaCellsByType(type)))
                       .ToDictionary(pair => pair.type, pair => pair.Item2);
        }
        private List<GridCell> GetPlacementAreaCellsByType(UnitType unitType)
        {
            return unitType switch {
                    UnitType.PLAYER => GetPlayerPlacementCells(),
                    UnitType.AI => GetEnemyPlacementCells(),
                    _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }
        public IReadOnlyCollection<GridCell> AllCells => _allCells ??= GetComponentsInChildren<GridCell>();
        private IReadOnlyDictionary<UnitType, List<GridCell>> PlacementAreaCells => _placementAreaCells ??= GetPlacementAreaCells();
        private List<GridCell> GetPlayerPlacementCells() => AllCells.Where(it => it.Id.Y < _config.UnitRowsCount).ToList();      
        private List<GridCell> GetEnemyPlacementCells() => AllCells.Where(it => it.Id.Y >= _config.Dimensions.y - _config.UnitRowsCount).ToList();


        public CellId GetCellByPos(Vector3 pos)
        {
            //TODO: calculate directly
            return AllCells.OrderBy(cell => Vector3Ext.DistanceXZ(pos, cell.transform.position)).First().Id;
        }

        public int SizeX => (int)_config.Dimensions.x;
        public int SizeY => (int)_config.Dimensions.y;

        public Vector3 GetCellPos(CellId cellId) => GetCell(cellId).transform.position;
    }
}
