using System.Collections.Generic;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Service;
using LegionMaster.UpgradeUnit.Config;

namespace LegionMaster.Units.Model.Meta
{
    public class UnitAttackModel : IUnitAttackModel
    {
        private readonly AttackConfig _attackConfig;
        private readonly UpgradeLevelConfig _upgradeLevelConfig;
        private readonly UpgradeStarsConfig _upgradeStarsConfig;
        private readonly RankedUnitModel _rankedUnit;
        
        public UnitAttackModel(AttackConfig attackConfig, UpgradeStarsConfig upgradeStarsConfig, UpgradeLevelConfig upgradeLevelConfig, RankedUnitModel rankedUnit)
        {
            _attackConfig = attackConfig;
            _upgradeLevelConfig = upgradeLevelConfig;
            _upgradeStarsConfig = upgradeStarsConfig;
            _rankedUnit = rankedUnit;
        }
        public float GetDamageBuff(AttackType attackType) => _attackConfig.GetDamageBuffConfig(attackType).DamageBuff;
        public IReadOnlyCollection<AttackType> AttackTypes => _attackConfig.AttackTypes;
        public int Attack => _attackConfig.Attack + UnitParameterCalculator.CalculateAddedAttack(_upgradeStarsConfig, _upgradeLevelConfig, _rankedUnit);

        public int AttackRangeInCells => _attackConfig.AttackRangeInCells;
        public float AttackInterval => _attackConfig.AttackInterval;

        public float AttackSpeed => 1 / AttackInterval;

        public float CriticalChance => _attackConfig.CriticalChance;

        public float CriticalDamage => _attackConfig.CriticalDamage;
    }
}