using System.Collections.Generic;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Units.Component;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Zenject;

namespace LegionMaster.UI.Screen.Squad
{
    public class UnitCursor : MonoBehaviour
    {
        private const string UNITS_PANEL_LAYER_NAME = "UnitsPanel";
        
        public static readonly Vector3 UnitVerticalOffset = 0.06f * Vector3.up;
        
        [Tooltip("snap distance as multiplier of grid cell size")]
        [SerializeField] private float _snapMultiplier = 1.5f;
        [SerializeField] private UnitUiOverlay _unitUiOverlay;

        private GridSnapper _snapper;
        private Plane _placingPlane;
        private Vector3 _startDragPosition;
        private bool _initialized;

        [Inject] private LocationArena _locationArena;
        [Inject] private Analytics.Analytics _analytics;

        public GridCell SelectedCell { get; private set; }    
        public bool RemovingArea { get; private set; }

        public GameObject AttachedUnit { get; private set; }

        private ArenaGrid Grid => _locationArena.CurrentGrid;
        private Transform UnitRoot => _locationArena.CurrentPlayerSquadSetup.UnitCursorRoot;
        private Transform GroundPlane => _locationArena.GroundPlane;
        public bool IsUnitAttached => AttachedUnit != null;
        public void Init()
        {
            _snapper = new GridSnapper(Grid, _snapMultiplier * Grid.CellSize);
            var plane = GroundPlane;
            _placingPlane = new Plane(plane.up, plane.position);
            _initialized = true;
        }

        public void Attach(GameObject unit)
        {
            Assert.IsNull(AttachedUnit);
            SetSelectedPlayerCell(null);
            AttachedUnit = unit;
            AttachedUnit.transform.SetParent(UnitRoot, false);
            AttachedUnit.transform.localPosition = UnitVerticalOffset;
            _unitUiOverlay.AddUnit(unit);

            _startDragPosition = AttachedUnit.transform.position;
        }


        public void Detach()
        {
            if (AttachedUnit == null) return;
            _unitUiOverlay.RemoveUnit(AttachedUnit);
            AttachedUnit.transform.SetParent(_locationArena.CurrentPlayerSquadSetup.TempRoot, true);
            
            var distance = Vector3.Distance(AttachedUnit.transform.position, _startDragPosition);
            if (distance > Grid.CellSize) _analytics.PlaceUnitByDragRight();
            
            AttachedUnit = null;
            SetSelectedPlayerCell(null);
        }

        private void Update()
        {
            if (!Initialized) return;
            
            CheckCursorOverUnitsPanel();
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!_placingPlane.Raycast(ray, out var distance)) return;
            var pos = ray.GetPoint(distance);
          
            if (AttachedUnit != null) {
                SetSelectedPlayerCell(_snapper.Snap(pos, UnitType.PLAYER));
            }
            UnitRoot.position = pos;
        }

        private bool Initialized => _initialized && _locationArena.CurrentPlayerSquadSetup != null;

        private void CheckCursorOverUnitsPanel()
        {
            var pointerData = new PointerEventData(EventSystem.current) {
                    position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            RemovingArea = results.Exists(it=>it.gameObject.layer == LayerMask.NameToLayer(UNITS_PANEL_LAYER_NAME));
        }

        private void SetSelectedPlayerCell(GridCell cell)
        {
            if (SelectedCell != cell && SelectedCell != null) {
                SelectedCell.Highlight(CellHighlight.Available);
            }
            SelectedCell = cell;
            if (SelectedCell != null) {
                SelectedCell.Highlight(CellHighlight.Selected);
            }
        }
    }
}