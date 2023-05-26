using UnityEngine;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public class ElementalBullet : Bullet
    {
        protected override void TryHit(GameObject target, Vector3 hitPos, Vector3 collisionNorm)
        {
            HitCallback?.Invoke(target);
        }
    }
}