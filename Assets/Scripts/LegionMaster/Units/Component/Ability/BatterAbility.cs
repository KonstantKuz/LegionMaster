using System;
using System.Collections;
using LegionMaster.Location.GridArena;
using LegionMaster.NavMap.Service;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Ability
{
    [RequireComponent(typeof(AttackUnit))]
    public class BatterAbility : BaseAbility
    {
        [SerializeField] private float _abilityTime;

        private float _timer;
        private Coroutine _attackCoroutine;

        [Inject] private NavMapService _navMapService;
        [Inject] private TargetListProvider _targetListProvider;
        [Inject] private IGridPositionProvider _gridPositionProvider;

        public override bool ShouldStart()
        {
            return UnitWithEnergy.IsFull && Owner.UnitStateMachine.Target != null;
        }

        public override void StartAbility(Action endCallback)
        {
            base.StartAbility(endCallback);
            StopAttackCoroutine();
            _timer = 0;
            
            StartAnimationWithFireHandler(StartTornadoAttack);
        }

        private void StartTornadoAttack()
        {
            RemoveFireHandler(StartTornadoAttack);
            _attackCoroutine = StartCoroutine(TornadoAttack());
        }

        public override void OnTick()
        {
            if (_attackCoroutine == null)
            {
                return;
            }
            
            _timer += Time.deltaTime;
            if (_timer >= _abilityTime)
            {
                SignalAbilityFinished();    
            }
        }

        public override void StopAbility()
        {
            StopAttackCoroutine();
        }

        private IEnumerator TornadoAttack()
        {
            while (true)
            {
                AttackEnemiesInRange();
                yield return new WaitForSeconds(Owner.UnitModel.UnitAttack.AttackInterval);
            }
        }

        private void AttackEnemiesInRange()
        {
            var ownerCellId = _gridPositionProvider.GetCellByPos(Owner.transform.position);
            var searchRadius = Owner.UnitModel.UnitAttack.AttackRangeInCells;
            var enemies = _navMapService.NavMap.GetEnemiesInRange(ownerCellId, Owner.UnitType.ToEnemyCellState(), searchRadius, _targetListProvider);
            
            foreach (var target in enemies)
            {
                AttackUnit.Fire(target);
            }
        }

        private void StopAttackCoroutine()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }
}