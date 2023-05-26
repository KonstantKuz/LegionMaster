using System;
using LegionMaster.Units.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.Units.Component.HealthEnergy
{
    public class UnitWithHealth : MonoBehaviour, IDamageable, IHealthBarOwner
    {
        private ReactiveProperty<float> _currentHealth;
        public IObservable<float> CurrentValue => _currentHealth;
        public int MaxValue => _healthConfig.MaxHealth;
        public bool BarEnabled => true;
        public bool DamageEnabled { get; set; }
        public event Action OnDeath;
        public event Action OnDamageTaken;
        
        private IUnitHealthModel _healthConfig;
      
        public void Init(Unit unit)
        {
            _healthConfig = unit.UnitModel.UnitHealth;
            _currentHealth = new FloatReactiveProperty(_healthConfig.StartingHealth);
            DamageEnabled = true;
        }
        public void TakeDamage(float damage)
        {
            if (!DamageEnabled) {
                return;
            }
            ChangeHealth(-damage);
            LogDamage(damage);
            
            if (_currentHealth.Value <= 0) {
                Die();
                return;
            }     
            OnDamageTaken?.Invoke();
        
        }

        public void RecoverHealth(float delta)
        {
            if (!DamageEnabled) {
                return;
            }
            ChangeHealth(delta);
        }

        private void Die()
        {
            DamageEnabled = false;
            OnDeath?.Invoke();
            OnDeath = null;
            OnDamageTaken = null;
        }
        
        private void ChangeHealth(float delta)
        {
            _currentHealth.Value = Mathf.Min(Mathf.Max(0, _currentHealth.Value + delta), MaxValue);
        }
        private void OnDestroy()
        {
            OnDeath = null;
            OnDamageTaken = null;
        }
        private void LogDamage(float damage)
        {
#if UNITY_EDITOR            
            Debug.Log($"Damage: -" + damage + " CurrentHealth: " + _currentHealth.Value + " GameObj:= " + gameObject.name);
#endif            
        }
    
    }
}