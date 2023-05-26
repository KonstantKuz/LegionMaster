using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.Charge.Projectile;
using LegionMaster.Units.Component.Target;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Ability
{
    [RequireComponent(typeof(AttackUnit))]
    public class SniperAbility : BaseAbility
    {
        [SerializeField] private GameObject _targetVfxPrefab;
        [SerializeField] private SniperAbilityBullet _bullet;
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _vfxDisplayTime = 1.0f;
        
        [Inject] private LocationObjectFactory _objectFactory;
        [Inject] private TargetListProvider _targetListProvider;

        private readonly List<GameObject> _vfxList = new List<GameObject>();
        private SniperAbilityDamager _damager;

        private SniperAbilityDamager Damager => _damager ??= new SniperAbilityDamager(Owner);

        private void OnDisable()
        {
            RemoveVfx();
        }

        public override void StartAbility(Action endCallback)
        {
            base.StartAbility(endCallback);
            StartAnimationWithFireHandler(Fire);
            PrepareShoot();
        }

        private void Fire()
        {
            Shoot();
            SignalAbilityFinished();
        }

        private void PrepareShoot()
        {
            var targets = GetTargets();
            targets.ForEach(AddTargetVfx);
            StartCoroutine(RemoveVfxAfterTimeout());
        }

        private IEnumerator RemoveVfxAfterTimeout()
        {
            yield return new WaitForSeconds(_vfxDisplayTime);
            RemoveVfx();
        }

        private IEnumerable<ITarget> GetTargets()
        {
            return _targetListProvider.AllTargets
                .Where(it => it.IsAlive && it.UnitType != Owner.UnitType);
        }

        private void AddTargetVfx(ITarget it)
        {
            var vfx = Instantiate(_targetVfxPrefab, it.Root, false);
            SetWorldScaleToOne(vfx);
            _vfxList.Add(vfx);
        }

        private static void SetWorldScaleToOne(GameObject vfx)
        {
            var scale = vfx.transform.lossyScale;
            vfx.transform.localScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
        }

        public override void OnTick()
        {
        }

        public override void StopAbility()
        {
            RemoveFireHandler(Fire);
        }

        private void Shoot()
        {
            var bullet = _objectFactory.CreateObject(_bullet.gameObject).GetComponent<SniperAbilityBullet>();
            bullet.transform.position = _barrel.position;
            bullet.LaunchByMultipleTargets(GetTargets(), obj => { Damager.DoDamage(obj, false); });
        }

        private void RemoveVfx()
        {
            _vfxList.ForEach(Destroy);
            _vfxList.Clear();
        }
    }
}