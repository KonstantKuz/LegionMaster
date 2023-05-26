using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Location.Session.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Config;
using Zenject;

namespace LegionMaster.Units.Model
{
    [PublicAPI]
    public class UnitModelBuilder
    {
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private StringKeyedConfigCollection<UpgradeLevelConfig> _upgradeLevelStringKeyedConfig;
        [Inject] private UpgradeStarsConfigCollection _upgradeStarsConfig;
        public UnitModel BuildUnit(string unitId, [NotNull] InventoryUnit inventoryUnit, 
                                   [CanBeNull] UnitOverrideParams overrideParams)
        {
            var unitConfig = _unitCollectionConfig.GetUnitConfig(unitId);
            var upgradeStarsConfig = _upgradeStarsConfig.GetConfig(unitId);
            var upgradeLevelConfig = _upgradeLevelStringKeyedConfig.Get(unitId);
            return new UnitModel(unitConfig, upgradeStarsConfig, upgradeLevelConfig, inventoryUnit, overrideParams);
        }

        public UnitModel BuildUnit(string unitId, [NotNull] UnitUpgradeParams upgradeParams,
                                   [CanBeNull] UnitOverrideParams overrideParams)
        {
            return BuildUnit(unitId, InventoryUnit.FromUpgradeParams(unitId, upgradeParams), overrideParams);
        }
        public UnitModel BuildInitialUnit(string unitId)
        {
            return BuildUnit(unitId, new UnitUpgradeParams {
                    Level = UnitUpgradeParams.DEFAULT_LEVEL,
                    Star = _unitCollectionConfig.GetUnitConfig(unitId).RankConfig.StartingStars
            }, null);
        }
    }
}