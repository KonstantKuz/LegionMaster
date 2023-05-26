using System;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Component.Weapon;
using LegionMaster.Units.Model;
using ModestTree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LegionMaster.Units.Component.Attack
{
    [RequireComponent(typeof(IDamager))]
    public class AttackUnit : MonoBehaviour, IInitializableComponent
    {
        private IUnitAttackModel _attackConfig;
        public event Action OnAttacked;
        public BaseWeapon Weapon { get; set; }
        public IDamager Damager { get; set; }

        public void Init(Unit unit)
        {
            _attackConfig = unit.UnitModel.UnitAttack;
            Weapon = GetComponentInChildren<BaseWeapon>();
            Damager = GetComponent<Damager>();
            Assert.IsNotNull(Weapon, "Unit prefab is missing BaseWeapon component in hierarchy"); 
            Assert.IsNotNull(Damager, "Unit prefab is missing Damager component in hierarchy");
        }
        public void Fire(ITarget target)
        {
            Assert.IsNotNull(Weapon, "BaseWeapon is missing"); 
            var isCritical = IsCriticalAttack();
            CustomFire(target, Weapon, isCritical);
        }
        public void CustomFire(ITarget target, BaseWeapon weapon, bool isCritical)
        {
            Assert.IsNotNull(Damager, "Damager is missing"); 
            weapon.Fire(target, obj => Damager.DoDamage(obj, isCritical));
            OnAttacked?.Invoke();
        }
        private bool IsCriticalAttack()
        {
            float probabilityGenerated = Random.Range(0, 100);
            return probabilityGenerated <= _attackConfig.CriticalChance;
        }
    }
}