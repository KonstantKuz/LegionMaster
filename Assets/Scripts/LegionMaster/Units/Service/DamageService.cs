using System;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Model;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace LegionMaster.Units.Service
{
    public class DamageService
    {
        public static void DoDamage(IUnitAttackModel shooterAttackModel, Unit targetUnit, bool critical)
        {
            var targetModel = targetUnit.UnitModel;
            Assert.IsNotNull(targetModel, $"UnitModel is null, gameObject:= {targetUnit.name}");

            var probabilityGenerated = Random.Range(0, 100);
            if (probabilityGenerated <= targetModel.DodgeChance) {
                Debug.Log($"No damage taken, target dodged, probabilityGenerated= {probabilityGenerated}, DodgeChance= {targetModel.DodgeChance}, target= {targetUnit.name}");
                return;
            }
            var damage = CalculateDamage(shooterAttackModel, targetModel, critical);
            if (damage <= 0) {
                return;
            }
            ApplyDamage(targetUnit, damage);
        }

        private static void ApplyDamage(Unit targetUnit, float damage)
        {
            var damageable = targetUnit.GetComponent<IDamageable>();
            Assert.IsNotNull(damageable, $"IDamageable is null, target= {targetUnit.name}");
            damageable.TakeDamage(damage);
        }
        private static float CalculateDamage(IUnitAttackModel shooterAttackModel, IUnitModel targetModel, bool critical)
        {
            float fullDamage = 0;
            foreach (var attackType in shooterAttackModel.AttackTypes) {
                fullDamage += CalculateDamageForAttackType(shooterAttackModel, targetModel, attackType);
            }
            if (critical) {
                fullDamage *= shooterAttackModel.CriticalDamage;
            }
            var damage = fullDamage - targetModel.Armor;
            return Math.Max(0, damage);
        }

        private static float CalculateDamageForAttackType(IUnitAttackModel shooterAttackModel, IUnitModel targetModel, AttackType attackType)
        {
            var shooterDamageBuf = shooterAttackModel.GetDamageBuff(attackType);
            var targetResistance = targetModel.GetResistance(attackType);
            var damage = shooterAttackModel.Attack * shooterDamageBuf * (1.0f - targetResistance);
            return Math.Max(0, damage);
        }
    }
}