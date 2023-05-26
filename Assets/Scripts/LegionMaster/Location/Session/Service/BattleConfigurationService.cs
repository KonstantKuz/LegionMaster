using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Core.Mode;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Session.Config;
using LegionMaster.Location.Session.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.Units.Component;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Battle;
using LegionMaster.Units.Model.Meta;
using LegionMaster.Units.Service;
using SuperMaxim.Core.Extensions;
using Zenject;

namespace LegionMaster.Location.Session.Service
{
    [PublicAPI]
    public class BattleConfigurationService
    {
        [Inject]
        private LocationArena _locationArena;
        [Inject]
        private UnitFactory _unitFactory;
        [Inject]
        private PlayerSquadService _playerSquadService;
        [Inject]
        private UnitModelBuilder _unitModelBuilder;
        [Inject]
        private InventoryService _inventoryService;

        public void Configure(BattleSession session)
        {
            var enemySpawnConfigs = session.EnemySquad.EnemySpawns.Select(it => it.ToUnitSpawnConfig(_locationArena.CurrentGrid.Config));

            var playerSquad = _playerSquadService.GetSquad(session.Mode).Value;
            SpawnUnitsInArena(playerSquad.Units, UnitType.PLAYER, session.Mode);
            SpawnUnitsInArena(enemySpawnConfigs, UnitType.AI, session.Mode);
        }
        private void SpawnUnitsInArena(IEnumerable<UnitSpawnConfig> unitSpawnConfigs, UnitType unitType, GameMode mode)
        {
            unitSpawnConfigs.ForEach(it => SpawnUnit(it, unitType, mode));
        }
        private void SpawnUnit(UnitSpawnConfig spawnConfig, UnitType unitType, GameMode mode)
        {
            var spawnPosition = _locationArena.CurrentGrid.GetCell(spawnConfig.CellId).transform;
            
            var model = unitType == UnitType.PLAYER ? GetPlayerUnitModel(spawnConfig, mode) : 
                                _unitModelBuilder.BuildUnit(spawnConfig.UnitId, spawnConfig.UpgradeParams, spawnConfig.OverrideParams);
            var battleModel = new UnitBattleModel(model)
            {
                    StarBarVisible = mode.IsMatchMode(),
            };
            _unitFactory.LoadForBattle(battleModel, unitType, spawnPosition);
        }
        
        private UnitModel GetPlayerUnitModel(UnitSpawnConfig spawnConfig, GameMode mode)
        {
            if (mode == GameMode.HyperCasual)
            {
                return _unitModelBuilder.BuildInitialUnit(spawnConfig.UnitId);
            }
            
            var inventoryUnit = _inventoryService.GetUnit(spawnConfig.UnitId);
            return mode.IsMatchMode() ? BuildUnitWithUpgrades(spawnConfig, inventoryUnit) : inventoryUnit;
        }

        private UnitModel BuildUnitWithUpgrades(UnitSpawnConfig spawnConfig, UnitModel inventoryUnit)
        {
            return _unitModelBuilder.BuildUnit(spawnConfig.UnitId, new UnitUpgradeParams() {
                    Level = inventoryUnit.InventoryUnit.Level,
                    Star = spawnConfig.UpgradeParams.Star,
            }, null);
        }
    }
}