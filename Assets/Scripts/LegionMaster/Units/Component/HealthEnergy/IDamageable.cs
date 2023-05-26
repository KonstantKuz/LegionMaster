using System;

namespace LegionMaster.Units.Component.HealthEnergy
{
    public interface IDamageable
    { 
        void TakeDamage(float damage);
        event Action OnDeath;
        event Action OnDamageTaken;
        bool DamageEnabled { get; set; } 
    }
}