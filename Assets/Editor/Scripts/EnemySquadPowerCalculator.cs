using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Config.Serializers;
using LegionMaster.Enemy.Config;
using LegionMaster.Location.Session.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Config;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
    public class EnemySquadPowerCalculator
    {
        private const string PATH_CONFIG_PREFIX = "Configs/";

        private const int GRID_SIZE_Y = 8;
   
        [MenuItem("LM/Calculate Enemy Power")]
        public static void CalculateEnemyPower()
        {
            var parser = new CsvConfigDeserializer();
            var battleConfig = ConfigLoader.LoadConfig<EnemiesConfig>(PATH_CONFIG_PREFIX + Configs.ENEMIES, parser);
            var unitCollectionConfig = ConfigLoader.LoadConfig<UnitCollectionConfig>(PATH_CONFIG_PREFIX + Configs.UNIT, parser);
            var upgradeLevelConfig = ConfigLoader.LoadConfig<StringKeyedConfigCollection<UpgradeLevelConfig>>(PATH_CONFIG_PREFIX + Configs.UNIT_UPGRADE_LEVEL, parser);
            var upgradeStarsConfig = ConfigLoader.LoadConfig<UpgradeStarsConfigCollection>(PATH_CONFIG_PREFIX + Configs.UNIT_UPGRADE_STARS, parser);
            
            foreach (var squadConfig in battleConfig.EnemySquadConfigs) {
                int enemySquadPower = CalculateSquadPower(squadConfig.EnemySpawns.Select(it => it.ToUnitSpawnConfig(GRID_SIZE_Y)), 
                                                          unitCollectionConfig, 
                                                          upgradeLevelConfig,
                                                          upgradeStarsConfig); 
                DisplayEnemySquadPower(squadConfig.Id, enemySquadPower);
            }
        }
        private static int CalculateSquadPower(IEnumerable<UnitSpawnConfig> spawnConfigs, 
                                               UnitCollectionConfig unitCollectionConfig, 
                                               StringKeyedConfigCollection<UpgradeLevelConfig> upgradeLevelStringKeyedConfig, 
                                               UpgradeStarsConfigCollection upgradeStarsConfig)
        {
            return spawnConfigs.Select(it => BuildUnitModel(it, unitCollectionConfig, upgradeLevelStringKeyedConfig, upgradeStarsConfig))
                               .Select(it => it.Power)
                               .Sum();
        }

        private static UnitModel BuildUnitModel(UnitSpawnConfig spawnConfig, 
                                                UnitCollectionConfig unitCollectionConfig, 
                                                StringKeyedConfigCollection<UpgradeLevelConfig> upgradeLevelStringKeyedConfig, 
                                                UpgradeStarsConfigCollection upgradeStarsConfig)
        {
            return new UnitModel(unitCollectionConfig.GetUnitConfig(spawnConfig.UnitId),
                                 upgradeStarsConfig.GetConfig(spawnConfig.UnitId), 
                                 upgradeLevelStringKeyedConfig.Get(spawnConfig.UnitId), 
                                 InventoryUnit.FromUpgradeParams(spawnConfig.UnitId, spawnConfig.UpgradeParams), spawnConfig.OverrideParams);
        }

        private static void DisplayEnemySquadPower(int squadId, int power)
        {
            Debug.Log($"SquadId:= {squadId}, power= {power}");
        }
    }
}