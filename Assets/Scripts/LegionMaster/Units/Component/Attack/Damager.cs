using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using ModestTree;
using UnityEngine;

namespace LegionMaster.Units.Component.Attack
{
    public class Damager : MonoBehaviour, IInitializableComponent, IDamager
    { 
        private IUnitAttackModel _attackConfig;
        public void Init(Unit unit)
        {
            _attackConfig = unit.UnitModel.UnitAttack;
        }
        public void DoDamage(GameObject target, bool isCritical)
        {
            var targetUnit = target.GetComponent<Unit>();
            if (targetUnit == null) return;
            Assert.IsNotNull(targetUnit, $"Unit is null, gameObject:= {target.name}");
            DamageService.DoDamage(_attackConfig, targetUnit, isCritical);
        }

    }
}