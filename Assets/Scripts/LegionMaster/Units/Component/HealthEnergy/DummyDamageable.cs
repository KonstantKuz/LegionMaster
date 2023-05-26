using System;
using UnityEngine;

namespace LegionMaster.Units.Component.HealthEnergy
{
    public class DummyDamageable : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float damage)
        {
        }

        public event Action OnDeath = delegate { };
        public event Action OnDamageTaken = delegate { };
        public bool DamageEnabled { get; set; }
    }
}