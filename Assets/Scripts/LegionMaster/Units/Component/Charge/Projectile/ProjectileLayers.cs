using UnityEngine;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public static class ProjectileLayers
    {
        public static int PlayerBullet => LayerMask.NameToLayer("PlayerBullet");
        public static int EnemyBullet => LayerMask.NameToLayer("EnemyBullet");
    }
}