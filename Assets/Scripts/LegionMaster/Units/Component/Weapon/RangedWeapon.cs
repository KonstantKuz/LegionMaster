﻿using System;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Charge.Projectile;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace LegionMaster.Units.Component.Weapon
{
    public class RangedWeapon : BaseWeapon
    {
        [SerializeField] 
        private Transform _barrel;
        [SerializeField] 
        private Projectile _ammo;
        [SerializeField]
        private float _recoilAngle;
        [Inject]
        private LocationObjectFactory _objectFactory;
    
        public override void Fire(ITarget target, Action<GameObject> hitCallback)
        {
            var projectile = CreateProjectile();
            var pos = _barrel.position;
            var rotationToTarget = GetShootRotation(pos, target.Center.position);
            rotationToTarget = AddRecoil(rotationToTarget);
            
            projectile.transform.SetPositionAndRotation(pos, rotationToTarget);
            projectile.Launch(target, hitCallback);
        }
        
        public static Quaternion GetShootRotation(Vector3 shootPos, Vector3 targetPos)
        {
            return Quaternion.LookRotation(GetShootDirection(shootPos, targetPos));
        }
        private static Vector3 GetShootDirection(Vector3 shootPos, Vector3 targetPos)
        {
            var dir = targetPos - shootPos;
            dir = new Vector3(dir.x, 0, dir.z);
            return dir.normalized;
        }
        private Quaternion AddRecoil(Quaternion rotationToTarget)
        {
            rotationToTarget *= Quaternion.Euler(Random.Range(-_recoilAngle, _recoilAngle), 0, Random.Range(0, 360));
            return rotationToTarget;
        }
        private Projectile CreateProjectile()
        {
            return _objectFactory.CreateObject(_ammo.gameObject).GetComponent<Projectile>();
        }
    }
}