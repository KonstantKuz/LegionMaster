using LegionMaster.Extension;
using UnityEngine;
using UnityEngine.Assertions;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class AttackState : BaseState
        {
            private float _lastAttackTime;
            private readonly int _attackHash = Animator.StringToHash("Attack");

            public AttackState(UnitStateMachine stateMachine) : base(stateMachine)
            {
            }

            public override void OnEnterState()
            {
                Assert.IsFalse(StateMachine.IsTargetInvalid);
                if (HasWeaponAnimationHandler)
                {
                    StateMachine._weaponAnimationHandler.FireEvent += Fire;
                }
            }

            public override void OnExitState()
            {
                if (HasWeaponAnimationHandler)
                {
                    StateMachine._weaponAnimationHandler.FireEvent -= Fire;
                }
            }

            public override void OnTick()
            {
                if (StateMachine.AbilityStart())
                {
                    return;
                }
                if (StateMachine.IsTargetInvalid || !IsTargetInRange())
                {
                    StateMachine.SetState(new IdleState(StateMachine));
                    return;
                }
                
                RotateTo(StateMachine.Target.Root.position);

                if (IsReady)
                {
                    Attack();
                }
            }
            
            private void Attack()
            {
                _lastAttackTime = Time.time;            
                if (!HasWeaponAnimationHandler)
                {
                    Fire();
                }
                StateMachine._animator.SetTrigger(_attackHash);                
            }
            
            private void Fire()
            {
                if (StateMachine.IsTargetInvalid) return;
                StateMachine._attackUnit.Fire(StateMachine.Target);
            }
            
            private void RotateTo(Vector3 targetPos)
            {
                var transform = StateMachine.transform;
                var lookAtDirection = (targetPos - transform.position).XZ().normalized;
                var lookAt = Quaternion.LookRotation(lookAtDirection, transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, Time.deltaTime * StateMachine._rotationSpeed);
            }   
            
            private bool IsTargetInRange()
            {
                //TODO: we should directly check target cell here
                var targetCellId = StateMachine._gridPositionProvider.GetCellByPos(StateMachine.Target.Root.position);
                var dist = Mathf.Max(Mathf.Abs(targetCellId.X - StateMachine.CurrentCellId.X), Mathf.Abs(targetCellId.Y - StateMachine.CurrentCellId.Y));
                return dist <= StateMachine.AttackRangeInCells;
            }      
            private bool IsReady => Time.time >= _lastAttackTime + StateMachine.AttackInterval;
            private bool HasWeaponAnimationHandler => StateMachine._weaponAnimationHandler != null;
        }
    }
}