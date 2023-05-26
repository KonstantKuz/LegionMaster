using LegionMaster.Units.Component.HealthEnergy;
using UnityEngine;

namespace LegionMaster.Units.Component
{
    public class UnitCollision : MonoBehaviour, IInitializableComponent
    {
        private Collider[] _colliders;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>();
        }

        public void Init(Unit unit)
        {
            SetCollidersEnabled(true);
            
            var damagable = GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.OnDeath += () => SetCollidersEnabled(false);
            }
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            foreach (var collider in _colliders) {
                collider.enabled = isEnabled;
            }
        }
    }
}