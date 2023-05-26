using System;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Charge.Projectile
{
    public class Grenade : Projectile
    {
        [SerializeField]
        private AnimationCurve _animationCurveJump;
        [SerializeField]
        private float _damageRadius;
        [SerializeField] 
        private float _speed;
        [SerializeField]
        private Explosion _explosion;
        [SerializeField] 
        private float _throwHeightScale;
        
        [Inject]
        private LocationObjectFactory _objectFactory;

        private float _flyTime;
        private float _maxFlyTime;
        private Vector3 _startPos;
        private Vector3 _endPos;

        public override void Launch(ITarget target, Action<GameObject> hitCallback)
        {
            base.Launch(target, hitCallback);
            _startPos = transform.position;
            _endPos =  target.Center.position;
            
            _flyTime = 0;
            _maxFlyTime = GetFlightTime(_startPos, _endPos);
        }

        private float GetFlightTime(Vector3 start, Vector3 end)
        {
            var xzPath = end - start;
            xzPath = new Vector3(xzPath.x, 0, xzPath.z);
            return xzPath.magnitude / _speed;
        }

        private void Update()
        {
            _flyTime = Mathf.Min(_flyTime + Time.deltaTime, _maxFlyTime);
            transform.position = GetPosition(_flyTime / _maxFlyTime);
            
            if (_flyTime < _maxFlyTime)
            {
                return;
            }

            Explode(transform.position);
        }

        private Vector3 GetPosition(float progress)
        {
            return Vector3.Lerp(_startPos, _endPos, progress) + _animationCurveJump.Evaluate(progress) * Vector3.up * _throwHeightScale;
        }

        private void Explode(Vector3 pos)
        {
            Explosion.Explode(_objectFactory, _explosion, pos, _damageRadius, TargetType, HitCallback);
            Destroy();
        }

        private void Destroy()
        {
            HitCallback = null;
            Destroy(gameObject);
        }
        
        protected override void TryHit(GameObject target, Vector3 hitPos, Vector3 collisionNorm)
        {
            Explode(hitPos);
        }
    }
}