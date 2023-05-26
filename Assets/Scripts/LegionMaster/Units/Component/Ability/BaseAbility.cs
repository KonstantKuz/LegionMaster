using System;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Weapon;
using UnityEngine;

namespace LegionMaster.Units.Component.Ability
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitWithEnergy))]
    public abstract class BaseAbility : MonoBehaviour
    {
        private static readonly int AbilityAnimationHash = Animator.StringToHash("Ability");
        
        private AttackUnit _attackUnit;
        private Animator _animator;
        private WeaponAnimationHandler _weaponAnimationHandler;
        
        private Action _endCallback;
        protected Unit Owner { get; private set; }
        protected UnitWithEnergy UnitWithEnergy { get; private set; }
        protected AttackUnit AttackUnit => _attackUnit ??= Owner.GetComponent<AttackUnit>();
        private Animator Animator => _animator ??= Owner.GetComponentInChildren<Animator>();

        private WeaponAnimationHandler WeaponAnimationHandler =>
            _weaponAnimationHandler ??= Owner.GetComponentInChildren<WeaponAnimationHandler>();
        

        protected virtual void Awake()
        {
            Owner = GetComponent<Unit>();
            UnitWithEnergy = GetComponent<UnitWithEnergy>();
        }

        public virtual bool ShouldStart() => UnitWithEnergy.IsFull;

        public virtual void StartAbility(Action endCallback)
        {
            _endCallback = endCallback;
            UnitWithEnergy.TakeAll();
        }
        
        public abstract void OnTick();
        public abstract void StopAbility();

        protected void SignalAbilityFinished()
        {
            _endCallback?.Invoke();
            _endCallback = null;
        }

        protected void StartAnimationWithFireHandler(Action handlerFunc)
        {
            Animator.SetTrigger(AbilityAnimationHash);
            WeaponAnimationHandler.FireEvent += handlerFunc;
        }

        protected void RemoveFireHandler(Action handlerFunc)
        {
            WeaponAnimationHandler.FireEvent -= handlerFunc;
        }
    }
}