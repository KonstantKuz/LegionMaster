using System;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using UnityEngine.Assertions;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Action<GameObject> HitCallback;
        protected UnitType TargetType;
        
        public virtual void Launch(ITarget target, Action<GameObject> hitCallback)
        {
            Assert.IsNotNull(target);
            TargetType = target.UnitType;     
            HitCallback = hitCallback;
            gameObject.layer = GetBulletLayer(target.UnitType); 
        }
        
        private static int GetBulletLayer(UnitType targetUnitType)
        {
            return targetUnitType switch
            {
                    UnitType.AI => ProjectileLayers.PlayerBullet,
                    UnitType.PLAYER => ProjectileLayers.EnemyBullet,
                    _ => throw new ArgumentOutOfRangeException(nameof(targetUnitType), targetUnitType, null)
            };
        }
        private void OnCollisionEnter(Collision other)
        {
            var colliderTarget = other.collider.GetComponent<ITarget>();
            if (colliderTarget == null) {
                return;
            }
            if (TargetType != colliderTarget.UnitType) {
                return;
            }
            
            if (!other.collider.TryGetComponent(out IDamageable damageable)) {
                return;
            }

            var contact = other.GetContact(0);
            TryHit(other.gameObject, contact.point, contact.normal);
        }

        protected abstract void TryHit(GameObject target, Vector3 hitPos, Vector3 collisionNorm);

    }
}