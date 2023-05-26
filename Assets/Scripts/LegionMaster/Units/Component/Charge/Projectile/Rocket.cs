using System;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public sealed class Rocket : Projectile
    {
        [SerializeField]
        private float _damageRadius;
        [SerializeField] 
        private float _speed;
        [SerializeField] 
        private bool _followTarget;
        [SerializeField] 
        private float _rotationSpeed;
        [SerializeField] 
        private float _maxLifeTime;
        [SerializeField]
        private Explosion _explosion;
        [SerializeField] 
        private float _selfExplodeRange;
        [SerializeField] 
        private float _initialCourseTime;
        
        [Inject]
        private LocationObjectFactory _objectFactory;

        private ITarget _target;
        private Vector3 _lastTargetPos;

        public float TimeLeft { get; private set; }

        public override void Launch(ITarget target, Action<GameObject> hitCallback)
        {
            base.Launch(target, hitCallback);
            SetTarget(target);
            TimeLeft = _maxLifeTime;
        }

        private void SetTarget(ITarget target)
        {
            Assert.IsNotNull(target);
            if (!_followTarget)
            {
                _lastTargetPos = target.Center.position;
                return;
            }

            if (_target == target) return;
            if (_target != null)
            {
                ClearTarget();
            }
            
            _target = target;
            _target.OnTargetInvalid += ClearTarget;
        }

        private void Update()
        {
            UpdateTargetPos();
            UpdatePosition();
            
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0 || Vector3.Distance(transform.position, _lastTargetPos) < _selfExplodeRange) {
                Explode(transform.position);
            }
        }

        private void UpdateTargetPos()
        {
            if (!_followTarget) return;
            if (!(_target is { IsAlive: true })) return;
            _lastTargetPos = _target.Center.position;
        }

        private void UpdatePosition()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;

            if (LifeTime >= _initialCourseTime)
            {
                var lookRotation = Quaternion.LookRotation(_lastTargetPos - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        private float LifeTime => _maxLifeTime - TimeLeft;

        protected override void TryHit(GameObject target, Vector3 hitPos, Vector3 collisionNorm)
        {
            Explode(hitPos);
        }
        private void Explode(Vector3 pos)
        {
            Explosion.Explode(_objectFactory, _explosion, pos, _damageRadius, TargetType, HitCallback);
            Destroy();
        }

        private void Destroy()
        {
            ClearTarget();
            HitCallback = null;
            Destroy(gameObject);
        }

        private void ClearTarget()
        {
            if (_target != null)
            {
                _target.OnTargetInvalid -= ClearTarget;
            }
            _target = null;
        }
    }
}