using System.Linq;
using UnityEngine;

namespace LegionMaster.Units.Component.Target
{
    public class RadialSearchTargetProvider : MonoBehaviour, ITargetProvider, IUpdatableUnitComponent, IInitializableComponent
    {
        [SerializeField]
        private float _searchRadius;
        [SerializeField]
        private float _updatePeriod = 0.5f;
        
        private float _lastUpdateTime;
        private UnitType _unitType;

        public ITarget Target { get; private set; }

        public void Init(Unit unit)
        {
            _unitType = unit.UnitType;
        }

        public void UpdateComponent()
        {
            if (Time.time < _lastUpdateTime + _updatePeriod) {
                return;
            }
            _lastUpdateTime = Time.time;
            
            var newTarget = FindTarget();
            SetTarget(newTarget);
        }

        private void SetTarget(ITarget newTarget)
        {
            if (Target != null)
            {
                Target.OnTargetInvalid -= OnTargetInvalid;
            }

            Target = newTarget;
            if (Target != null)
            {
                Target.OnTargetInvalid += OnTargetInvalid;
            }
        }

        private ITarget FindTarget()
        {
            return Physics.OverlapSphere(transform.position, _searchRadius)
                .Select( it => it.GetComponent<ITarget>())
                .Where(it => it != null)
                .Where(it => it.IsAlive && it.UnitType != _unitType)
                .OrderBy(it => (it.Center.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void OnTargetInvalid()
        {
            SetTarget(null);
            _lastUpdateTime = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _searchRadius);
        }
    }
}