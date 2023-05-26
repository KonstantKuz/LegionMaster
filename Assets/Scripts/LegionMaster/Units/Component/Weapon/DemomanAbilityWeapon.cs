using System;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Charge.Projectile;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Weapon
{
    public class DemomanAbilityWeapon : BaseWeapon
    {
        [SerializeField] 
        private Transform _barrel;
        [SerializeField] 
        private Projectile _ammo;
        [SerializeField] 
        private float _upCoefficient;
        
        [Inject]
        private LocationObjectFactory _objectFactory;

        public override void Fire(ITarget target, Action<GameObject> hitCallback)
        {
            var projectile = CreateProjectile();
            var pos = _barrel.position;
            var rotationToTarget = Quaternion.Lerp(
                RangedWeapon.GetShootRotation(pos, target.Center.position),
                Quaternion.LookRotation(Vector3.up),  //TODO; take direction from animation (barrel direction) when animation will be done
                _upCoefficient);

            projectile.transform.SetPositionAndRotation(pos, rotationToTarget);
            projectile.Launch(target, hitCallback);
        }
        
        private Projectile CreateProjectile()
        {
            return _objectFactory.CreateObject(_ammo.gameObject).GetComponent<Projectile>();
        }
    }
}