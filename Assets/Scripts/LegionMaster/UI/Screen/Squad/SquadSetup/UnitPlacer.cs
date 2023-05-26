using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.UI.Screen.Squad.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.SquadSetup
{
    public class UnitPlacer
    {
        private PlayerSquadSetup _playerSquadSetup;
        private UnitCursor _unitCursor;

        public UnitPlacer(PlayerSquadSetup playerSquadSetup, UnitCursor unitCursor)
        {
            _playerSquadSetup = playerSquadSetup;
            _unitCursor = unitCursor;
            _unitCursor.Init();
        }
        public GameObject PlaceInCellNewUnit(PlacedUnitModel newUnitModel, GridCell cell)
        {
            if (_unitCursor.AttachedUnit) {
                ClearCursor();
            }
            var unitGameObject = _playerSquadSetup.LoadUnit(newUnitModel);
            _playerSquadSetup.PlaceUnit(unitGameObject, cell);
            SetCellId(newUnitModel, cell.Id);
            return unitGameObject;
        }

        public void PlaceInCell(PlacedUnitModel unit, GridCell cell)
        {
            var unitObject = _unitCursor.AttachedUnit;
            _unitCursor.Detach();
            if (cell == null) {
                SetCellId(unit, CellId.InvalidCellId);
                _playerSquadSetup.DestroyUnit(unitObject);
                return;
            }
            if (cell.IsFull) {
                Swap(unitObject, unit, cell);
                return;
            }
            _playerSquadSetup.PlaceUnit(unitObject, cell);
            SetCellId(unit, cell.Id);
        }
        public void SetCellId(PlacedUnitModel unit, CellId cellId)
        {
            unit.CellId = cellId;
            _playerSquadSetup.Save();
        }

        private void Swap(GameObject attachedUnitObject, PlacedUnitModel attachedUnit, GridCell highlightCell)
        {
            var standingUnit = _playerSquadSetup.GetPlacedUnitByCellId(highlightCell.Id);

            var previousGridCell = _playerSquadSetup.Grid.GetCell(attachedUnit.CellId);
            _playerSquadSetup.PlaceUnit(_playerSquadSetup.GetUnitObject(standingUnit), previousGridCell);
            SetCellId(standingUnit, previousGridCell.Id);

            _playerSquadSetup.PlaceUnit(attachedUnitObject, highlightCell);
            SetCellId(attachedUnit, highlightCell.Id);
        }
        public void ClearCursor()
        {
            if (!_unitCursor.AttachedUnit) {
                return;
            }
            var unitObject = _unitCursor.AttachedUnit;
            _unitCursor.Detach();
            _playerSquadSetup.DestroyUnit(unitObject);
        }
    }
}