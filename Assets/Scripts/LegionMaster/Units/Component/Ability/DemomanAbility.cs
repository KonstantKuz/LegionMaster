using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Component.Weapon;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Ability
{
    [RequireComponent(typeof(AttackUnit))]
    public class DemomanAbility : BaseAbility
    {
        [SerializeField] private float _shotInterval;
        [SerializeField] private BaseWeapon _weapon;

        private Coroutine _shootCoroutine;

        [Inject] private TargetListProvider _targetListProvider;

        public override void StartAbility(Action endCallback)
        {
            base.StartAbility(endCallback);
            StopShootCoroutine();
            StartAnimationWithFireHandler(Fire);
        }

        private void Fire()
        {
            var enemies = _targetListProvider.AllTargets
                .Where(it => it.IsAlive && it.UnitType != Owner.UnitType).ToList();
            _shootCoroutine = StartCoroutine(Shoot(enemies));
        }

        private IEnumerator Shoot(IEnumerable<ITarget> targets)
        {
            foreach (var target in targets.ToList())
            {
                if (!target.IsAlive) continue;
                AttackUnit.CustomFire(target, _weapon, true);
                yield return new WaitForSeconds(_shotInterval);
            }

            _shootCoroutine = null;
            SignalAbilityFinished();
        }

        public override void OnTick()
        {
        }

        public override void StopAbility()
        {
            RemoveFireHandler(Fire);
            StopShootCoroutine();
        }

        private void StopShootCoroutine()
        {
            if (_shootCoroutine != null)
            {
                StopCoroutine(_shootCoroutine);
                _shootCoroutine = null;
            }
        }
    }
}