using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Ai
{
    [CreateAssetMenu(fileName = "behavior", menuName = "LegionMaster/Ai/RangedBehavior")]
    public class RangedBehavior : AiBehaviorBase
    {
        [SerializeField] 
        private float _rotationSpeed;
        [SerializeField]
        private float _minDistanceToTarget;
        [SerializeField]
        private float _checkDistanceTimeout = 0.5f;
        [SerializeField] 
        private float _attackRangeMultiplier = 0.9f;

        private float _lastDistanceCheckTime;
        private ITarget _lastTarget;
        
        
        
        public override void ProcessTimer(AiUnit aiUnit)
        {
            var target = aiUnit.Target;
            if (_lastTarget != target)
            {
                ChangeTarget(target);
            }
            if (target == null) {
                aiUnit.SetMoving(false);
                return;
            }
            UpdateMovement(target.Root.position, aiUnit.transform.position, aiUnit);
            aiUnit.SetRotationEnabled(false);
            aiUnit.LookAtTarget(_rotationSpeed);
        }

        private void ChangeTarget(ITarget target)
        {
            _lastTarget = target;
            _lastDistanceCheckTime = 0;
        }

        private void UpdateMovement(Vector3 targetPos, Vector3 ourPos, AiUnit aiUnit)
        {
            var distanceToTarget = (ourPos - targetPos).magnitude;
            var dirToTarget = (targetPos - ourPos).normalized;
            var attackDistance = aiUnit.AttackRange * _attackRangeMultiplier;            

            if (distanceToTarget < _minDistanceToTarget)
            {
                RunAwayIfTooClose(aiUnit, targetPos, dirToTarget);
            } else if (distanceToTarget <= attackDistance) {
                aiUnit.SetMoving(false);
            } else
            {
                MoveTo(aiUnit, targetPos - attackDistance * dirToTarget);
            }
        }

        private void RunAwayIfTooClose(AiUnit aiUnit, Vector3 targetPos, Vector3 dirToTarget)
        {
            if (!ShouldRunAway()) return;
            MoveTo(aiUnit, targetPos - _minDistanceToTarget * dirToTarget);
            _lastDistanceCheckTime = Time.time;
        }

        private bool ShouldRunAway()
        {
            return Time.time - _lastDistanceCheckTime >= _checkDistanceTimeout;
        }

        private static void MoveTo(AiUnit aiUnit, Vector3 shootingPos)
        {
            aiUnit.SetMoving(true);
            aiUnit.SetDestination(shootingPos);
            aiUnit.MovementDirection = (shootingPos - aiUnit.transform.position).normalized;
        }
    }
}