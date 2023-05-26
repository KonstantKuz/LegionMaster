using System;
using System.Linq;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Config;
using UnityEngine;

namespace LegionMaster.Units.Service
{
    public class UnitParameterCalculator
    {
        private const int DEFAULT_LEVEL = 1;
        private const int DEFAULT_PARAMETERS_COUNT = 9;
        private const int POWER_COEFFICIENT = 100;
        private const int MIN_POWER_VALUE = 1;

        public static int CalculatePower(UnitModel unit)
        {
            var baseUnit = unit.BaseUnit;
            float armor = DivideParams(unit.Armor, baseUnit.Armor);
            float health = DivideParams(unit.UnitHealth.StartingHealth, baseUnit.HealthConfig.Health);
            float moveSpeed = DivideParams(unit.MoveSpeed.Value, baseUnit.MoveSpeed);
            float dodgeChance = DivideParams(unit.DodgeChance, baseUnit.DodgeChance);
            
            var attackUnit = unit.UnitAttack;
            var baseAttackUnit = baseUnit.AttackConfig;

            float attack = DivideParams(attackUnit.Attack, baseAttackUnit.Attack);
            float attackDistance = DivideParams(attackUnit.AttackRangeInCells, baseAttackUnit.AttackRangeInCells);
            
            float attackSpeed = DivideParams(attackUnit.AttackSpeed, baseAttackUnit.AttackSpeed);
            float criticalChance = DivideParams(attackUnit.CriticalChance, baseAttackUnit.CriticalChance);

            float criticalDamage = DivideParams(attackUnit.CriticalDamage, baseAttackUnit.CriticalDamage);

            float sumDamageBuff = GetSumDamageBuffParameters(attackUnit, baseAttackUnit);
            float sumResistance = GetSumResistanceParameters(unit, baseUnit);

            float sumParameters = armor + health + moveSpeed + dodgeChance + attack + attackDistance + attackSpeed + criticalChance + criticalDamage
                                  + sumDamageBuff + sumResistance;
            int parametersCount = DEFAULT_PARAMETERS_COUNT + Enum.GetValues(typeof(AttackType)).Length * 2;
            double power = (sumParameters / parametersCount) * POWER_COEFFICIENT;
            power = Math.Round(power, MidpointRounding.AwayFromZero);
            return Math.Max(MIN_POWER_VALUE, (int) power); 
        }

        private static float DivideParams(float currentParam, float baseParam)
        {
            if (baseParam == 0) {
                return 0;
            }
            return currentParam / baseParam;
        }

        private static float GetSumDamageBuffParameters(IUnitAttackModel unitAttack, AttackConfig baseAttackUnit)
        {
            float sumDamageBuff = 0;
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType))) {
                sumDamageBuff += DivideParams(unitAttack.GetDamageBuff(attackType), baseAttackUnit.GetDamageBuffConfig(attackType).DamageBuff);
            }
            return sumDamageBuff;
        }   
        private static float GetSumResistanceParameters(UnitModel unit, UnitConfig baseUnit)
        {
            float sumResistance = 0;
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType))) {
                sumResistance += DivideParams(unit.GetResistance(attackType), baseUnit.GetResistanceConfig(attackType).Resistance);
            }
            return sumResistance;
        }

        public static int CalculateAddedArmor(UpgradeStarsConfig starsConfig, UpgradeLevelConfig levelConfig, RankedUnitModel rankedUnit)
        {
            return levelConfig.UpgradeParametersConfig.Armor * GetAdditionalLevels(rankedUnit.Level) + 
                   starsConfig.GetUpgradeStarConfigsByStar(rankedUnit.Star).Sum(it => it.UpgradeParametersConfig.Armor);
        }    
        public static int CalculateAddedHealth(UpgradeStarsConfig starsConfig, UpgradeLevelConfig levelConfig, RankedUnitModel rankedUnit)
        {
            return levelConfig.UpgradeParametersConfig.Health * GetAdditionalLevels(rankedUnit.Level) + 
                   starsConfig.GetUpgradeStarConfigsByStar(rankedUnit.Star).Sum(it => it.UpgradeParametersConfig.Health);
        }  
        public static int CalculateAddedAttack(UpgradeStarsConfig starsConfig, UpgradeLevelConfig levelConfig, RankedUnitModel rankedUnit)
        {
            return levelConfig.UpgradeParametersConfig.Attack * GetAdditionalLevels(rankedUnit.Level) + 
                   starsConfig.GetUpgradeStarConfigsByStar(rankedUnit.Star).Sum(it => it.UpgradeParametersConfig.Attack);
        }
        private static int GetAdditionalLevels(int unitLevel) => Mathf.Max(0, unitLevel - DEFAULT_LEVEL);
    }
}