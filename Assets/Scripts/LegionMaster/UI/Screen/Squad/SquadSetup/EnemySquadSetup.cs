using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Extension;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena;
using LegionMaster.Location.Session.Config;
using LegionMaster.Location.Session.Service;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.SquadSetup
{
    public class EnemySquadSetup : MonoBehaviour
    {
        [SerializeField] private Transform _unitsRoot;

        [Inject] private UnitFactory _unitFactory;
        [Inject] private LocationArena _locationArena;
        [Inject] private UnitModelBuilder _unitModelBuilder;
        [Inject] private EnemySquadService _enemySquadService;
        [Inject] private BattleSessionServiceWrapper _sessionService;
        
        private bool _unitHudEnabled;
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        public bool CanShow(GameMode mode)
        {
            return mode != GameMode.Battle || _enemySquadService.BattleSquadExists;
        }

        private void OnEnable()
        {
            _unitsRoot.transform.DestroyAllChildren();
            LoadUnits();
        }
        private void OnDisable()
        {
            _unitsRoot.transform.DestroyAllChildren();
        }
        private void LoadUnits()
        {
            _sessionService.BattleSession.EnemySquad.EnemySpawns
                           .Select(it => it.ToUnitSpawnConfig(_locationArena.CurrentGrid.Config))
                           .ForEach(LoadAndPlaceUnit);
        }
        
        private void LoadAndPlaceUnit(UnitSpawnConfig unitSpawn)
        {
            var unit = _unitFactory.LoadUnitForUi(unitSpawn.UnitId, UnitType.AI, _unitsRoot);
            PlaceUnit(unit.gameObject, unitSpawn);
            var model = _unitModelBuilder.BuildUnit(unitSpawn.UnitId, unitSpawn.UpgradeParams, unitSpawn.OverrideParams);
            model.HudVisible = UnitHudEnabled;
            model.StarBarVisible = false;
            unit.Init(model);
        }
        private void PlaceUnit(GameObject unit, UnitSpawnConfig unitSpawn)
        {
            var cell = Grid.GetCell(unitSpawn.CellId);
            unit.transform.SetPositionAndRotation(cell.transform.position, UnitType.AI.GetSpawnRotation(cell.transform));
        }
        private ArenaGrid Grid => _locationArena.CurrentGrid;
        public bool UnitHudEnabled
        {
            get => _unitHudEnabled;
            set
            {
                _unitHudEnabled = value;
                _unitsRoot.gameObject.GetComponentsInChildren<Unit>().ForEach(unit => unit.UnitModel.HudVisible = _unitHudEnabled);
            }
        }
    }
}