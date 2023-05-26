using UnityEngine;

namespace LegionMaster.Units
{
    public class WaitForTargetReached : CustomYieldInstruction
    {
        private const float DISTANCE_PRECISION = 0.1f;
        
        private readonly Unit _unit;
        private readonly Vector3 _targetPosition;
        
        public override bool keepWaiting => Vector3.Distance(_unit.transform.position, _targetPosition) > DISTANCE_PRECISION;

        public WaitForTargetReached(Unit unit, Vector3 targetPosition)
        {
            _unit = unit;
            _targetPosition = targetPosition;
        }
    }
}