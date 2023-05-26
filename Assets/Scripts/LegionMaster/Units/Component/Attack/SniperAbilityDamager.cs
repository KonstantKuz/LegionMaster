using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using UnityEngine;
using UnityEngine.Assertions;

namespace LegionMaster.Units.Component.Attack
{
    public class SniperAbilityDamager : IDamager
    {
        private readonly IUnitAttackModel _attackConfig;
        private readonly Unit _owner;

        public SniperAbilityDamager(Unit unit)
        {
            _owner = unit;
            _attackConfig = unit.UnitModel.UnitAttack;
        }

        public void DoDamage(GameObject target, bool isCritical)
        {
            var targetUnit = target.GetComponent<Unit>();
            if (targetUnit == null) return;
            Assert.IsNotNull(targetUnit, $"Unit is null, gameObject:= {targetUnit.name}");
            DamageService.DoDamage(_attackConfig, targetUnit, isCritical);
            targetUnit.EffectOwner.AddEffect(EffectType.Stun, _owner);
        }
    }
}