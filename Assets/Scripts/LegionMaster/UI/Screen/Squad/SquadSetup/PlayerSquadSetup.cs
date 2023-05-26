using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Extension;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Player.Squad.Model;
using LegionMaster.Player.Squad.Service;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units.Component;
using LegionMaster.Units.Config;
using LegionMaster.Units.Service;
using ModestTree;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.SquadSetup
{
    public delegate void DragAction(PlacedUnitModel unit, GameObject obj);
    
    [Serializable]
    public abstract class PlayerSquadSetup : MonoBehaviour
    {
        [SerializeField] protected GameMode _gameMode;
        [SerializeField] private Transform _unitsRoot;
        [SerializeField] private Transform _unitCursorRoot;
        [SerializeField] private Transform _tempRoot;
        [SerializeField] private UnitColliderParams _colliderParams;
        
        [Inject] private PlayerSquadService _squadService;
        [Inject] private LocationArena _locationArena;
        [Inject] private UnitFactory _unitFactory;

        protected List<PlacedUnitModel> PlacedUnitModelList { get; set; }
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        
        private void Init()
        {
            RemoveAllUnits();
            CreateModelList();
            PlaceUnits();
        }
        private void OnEnable()
        {
            Grid.HighlightCells(UnitType.PLAYER, CellHighlight.Available);
            Init();
        }
        
        private void OnDisable()
        {
            Grid.HighlightCells(UnitType.PLAYER, CellHighlight.None);
            RemoveAllUnits();
        }
        
        public abstract GameObject LoadUnit(PlacedUnitModel model);
        public abstract GameObject GetUnitObject(PlacedUnitModel unit);
        public abstract void Save();
        
        protected abstract void CreateModelList();      
        protected abstract void PlaceUnits();
        
        public void AddToModelList(PlacedUnitModel unit) => PlacedUnitModelList.Add(unit);
        public void RemoveFromModelList(PlacedUnitModel unit) => PlacedUnitModelList.Remove(unit);
        
        public PlacedUnitModel GetPlacedUnitByCellId(CellId cellId)
        {
            return PlacedUnits.FirstOrDefault(it => it.CellId == cellId)
                   ?? throw new NullReferenceException($"PlacedUnitModel not found by cellId:= {cellId}");
        }
        public void PlaceUnit(GameObject unitObj, GridCell cell)
        {
            Assert.IsNotNull(cell);
            unitObj.transform.SetParent(_unitsRoot, true);
            PlaceUnitInCell(unitObj, cell);
        }
    
        public void DestroyUnit(GameObject unitObj) => Destroy(unitObj);

        public void SetCellFullState(PlacedUnitModel placedUnit, bool full) => GetGridCellById(placedUnit.CellId).IsFull = full;

        public GridCell GetGridCellById(CellId cellId) => Grid.GetCell(cellId);
        
        protected static CellId GetCellId(string unitId, SquadModel squad)
        {
            var place = squad.GetUnit(unitId);
            return place?.CellId ?? CellId.InvalidCellId;
        }
        protected GameObject LoadUnit(PlacedUnitModel model, DragAction onStartUnitDrag, DragAction onFinishUnitDrag)
        {
            return _unitFactory.LoadDraggablePlayerUnit(model.Id,
                                                       _unitsRoot, 
                                                       item => onStartUnitDrag?.Invoke(model, item),
                                                       item => onFinishUnitDrag?.Invoke(model, item),
                                                       _colliderParams).gameObject;
        }
        protected static void PlaceUnitInCell(GameObject obj, GridCell cell)
        {
            obj.transform.position = cell.transform.position + UnitCursor.UnitVerticalOffset;
            cell.IsFull = true;
        }
        private void RemoveAllUnits()
        {
            Grid.Reset();
            _unitsRoot.transform.DestroyAllChildren();  
            _tempRoot.transform.DestroyAllChildren();
            PlacedUnitModelList = null;
        }
        public IReadOnlyCollection<PlacedUnitModel> PlacedUnits => PlacedUnitModelList;  
        public ArenaGrid Grid => _locationArena.CurrentGrid;
        public GridCell RandomFreeCell => Grid.RandomFreeCell(UnitType.PLAYER);
        public Transform UnitCursorRoot => _unitCursorRoot;
        public Transform TempRoot => _tempRoot;
        public Transform UnitsRoot => _unitsRoot;
        
        protected PlayerSquadService SquadService => _squadService;
    }
}