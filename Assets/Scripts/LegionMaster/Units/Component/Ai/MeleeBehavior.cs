using UnityEngine;

namespace LegionMaster.Units.Component.Ai
{
    [CreateAssetMenu(fileName = "behavior", menuName = "LegionMaster/Ai/MeleeBehavior")]
    public class MeleeBehavior : AiBehaviorBase
    {
        [SerializeField] 
        private float _rotationSpeed;
        [SerializeField]
        private float _disableSmoothRotationDistance;
        [SerializeField] 
        private float _attackDistanceMultiplier = 0.9f;

        public override void ProcessTimer(AiUnit aiUnit)
        {
            var target = aiUnit.Target;
            if (target == null)
            {
                aiUnit.SetMoving(false);
                return;
            }

            var ourPos = aiUnit.transform.position;
            var targetPos = target.Root.position;
            var distanceToTarget = Vector3.Distance(ourPos, targetPos);
            var targetDir = (targetPos - ourPos).normalized;
            
            UpdateMovement(aiUnit, distanceToTarget, ourPos, targetDir);
            UpdateRotation(aiUnit, targetDir, distanceToTarget);
        }

        private void UpdateMovement(AiUnit aiUnit, float distanceToTarget, Vector3 ourPos, Vector3 targetDir)
        {
            var attackDistance = aiUnit.AttackRange * _attackDistanceMultiplier;
            if (distanceToTarget <= attackDistance)
            {
                aiUnit.SetMoving(false);
            }
            else
            {
                aiUnit.SetMoving(true);
                aiUnit.SetDestination(ourPos + targetDir * (distanceToTarget - attackDistance));
            }
        }

        private void UpdateRotation(AiUnit aiUnit, Vector3 targetDir, float distanceToTarget)
        {
            aiUnit.LookAtDirection = aiUnit.MovementDirection = targetDir;
            aiUnit.SetRotationEnabled(true);

            if (distanceToTarget >= _disableSmoothRotationDistance)
            {
                return;
            }

            aiUnit.SetRotationEnabled(false);
            aiUnit.LookAtTarget(_rotationSpeed);
        }
    }
}