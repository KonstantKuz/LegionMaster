using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using UnityEngine.Assertions;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public class SniperAbilityBullet : Bullet
    {
        [SerializeField] private float _destroyDelay = 0.5f;
        private List<ITarget> _targets;
        
        protected override void TryHit(GameObject target, Vector3 hitPos, Vector3 collisionNorm)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), target.GetComponent<Collider>());
            HitCallback?.Invoke(target);
            PlayVfx(hitPos, collisionNorm);
            ShootNextTarget();
        }
        
        public void LaunchByMultipleTargets(IEnumerable<ITarget> targets, Action<GameObject> hitCallback)
        {
            Assert.IsNull(_targets, "LaunchByMultipleTargets should be called only once per bullet");
            _targets = targets.ToList();
            HitCallback = hitCallback;
            SetupBullet();            
            ShootNextTarget();
        }

        private void ShootNextTarget()
        {
            var nextTarget = GetNextTarget();
            if (nextTarget == null)
            {
                StartCoroutine(DestroyWithDelay());
                return;
            }
            transform.rotation = Quaternion.LookRotation(GetShootDirection(transform.position, nextTarget.Center.position));    
            Launch(nextTarget, HitCallback);
        }

        private IEnumerator DestroyWithDelay()
        {
            DisablePhysic();
            yield return new WaitForSeconds(_destroyDelay);
            Destroy();
        }

        private void DisablePhysic()
        {
            Rigidbody.detectCollisions = false;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<Collider>().enabled = false;
        }

        private ITarget GetNextTarget()
        {
            var target = _targets.OrderBy(it => Vector3.Distance(transform.position, it.Center.position)).FirstOrDefault();
            _targets.Remove(target);
            return target;
        }
        
        private static Vector3 GetShootDirection(Vector3 shootPos, Vector3 targetPos)
        {
            var dir = targetPos - shootPos;
            dir = new Vector3(dir.x, 0, dir.z);
            return dir.normalized;
        }
    }
}