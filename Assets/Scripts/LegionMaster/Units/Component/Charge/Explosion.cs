using System;
using System.Linq;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Charge
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem[] _particles;

        public void Explode(float damageRadius, UnitType targetType, Action<GameObject> hitCallback)
        {
            PlayParticles();
            var hits = GetHits(damageRadius, targetType);
            DamageHits(hits, hitCallback);
        }

        private void PlayParticles()
        {
            for (int i = 0; i < _particles.Length; i++) {
                _particles[i].Play();
            }
        }

        private Collider[] GetHits(float damageRadius, UnitType targetType)
        {
            var hits = Physics.OverlapSphere(transform.position, damageRadius);
            return hits.Where(go => go.GetComponent<ITarget>() != null && go.GetComponent<ITarget>().IsAlive
                                    && go.GetComponent<ITarget>().UnitType == targetType)
                       .ToArray();
        }

        private void DamageHits(Collider[] hits, Action<GameObject> hitCallback)
        {
            foreach (Collider hit in hits) {
                if (hit.TryGetComponent(out IDamageable damageable)) {
                    hitCallback?.Invoke(hit.gameObject);
                }
            }
        }

        public static void Explode(LocationObjectFactory objectFactory, 
            Explosion prefab, 
            Vector3 pos,
            float radius, 
            UnitType targetType,
            Action<GameObject> hitCallback)
        {
            var explosion = objectFactory.CreateObject(prefab.gameObject).GetComponent<Explosion>();
            explosion.transform.position = pos;
            explosion.Explode(radius, targetType, hitCallback);            
        }
    }
}