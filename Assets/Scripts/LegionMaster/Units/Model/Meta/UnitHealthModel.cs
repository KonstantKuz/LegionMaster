using LegionMaster.Units.Config;
using LegionMaster.Units.Service;
using LegionMaster.UpgradeUnit.Config;

namespace LegionMaster.Units.Model.Meta
{
    public class UnitHealthModel : IUnitHealthModel
    {
        private readonly HealthConfig _healthConfig;
        private readonly UpgradeLevelConfig _upgradeLevelConfig;
        private readonly UpgradeStarsConfig _upgradeStarsConfig;
        private readonly RankedUnitModel _rankedUnit;

        public UnitHealthModel(HealthConfig healthConfig, UpgradeStarsConfig upgradeStarsConfig, UpgradeLevelConfig upgradeLevelConfig, RankedUnitModel rankedUnit)
        {
            _healthConfig = healthConfig;
            _upgradeLevelConfig = upgradeLevelConfig;
            _upgradeStarsConfig = upgradeStarsConfig;
            _rankedUnit = rankedUnit;
        }
        
        public int MaxHealth => _healthConfig.Health + UnitParameterCalculator.CalculateAddedHealth(_upgradeStarsConfig, _upgradeLevelConfig, _rankedUnit);
        public int StartingHealth => MaxHealth;
        
        public int RecoveryPerAttack => _healthConfig.RecoveryPerAttack;

        public int RecoveryPerHit => _healthConfig.RecoveryPerHit;

        public int RecoveryPerSecond => _healthConfig.RecoveryPerSecond;

        public int RecoveryPerDeath => _healthConfig.RecoveryPerDeath;
    }
}