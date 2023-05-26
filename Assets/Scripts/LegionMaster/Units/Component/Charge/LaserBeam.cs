using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Charge
{
    public class LaserBeam : Beam
    {
        [SerializeField] private GameObject _hitEffect;

        private LineRenderer _laser;
        private ParticleSystem[] _effects;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int Noise = Shader.PropertyToID("_Noise");

        private void Awake()
        {
            _laser = GetComponentInChildren<LineRenderer>();
            _effects = GetComponentsInChildren<ParticleSystem>();
        }

        protected override void SetTarget(ITarget target)
        {
            base.SetTarget(target);
            PlayAllEffects();
        }

        private void PlayAllEffects()
        {
            foreach (var ps in _effects)
            {
                ps.Play();
            }
        }

        protected override void ClearTarget()
        {
            StopAllEffects();
            base.ClearTarget();
        }

        protected override void Update()
        {
            base.Update();
            
            if (Target != null)
            {
                UpdateBeam(Target.Center.position);
            }

            _laser.enabled = Target != null;
        }

        private void UpdateBeam(Vector3 hitPoint)
        {
            UpdatePosition(hitPoint);
            UpdateTextureScale(hitPoint);
        }

        private void UpdateTextureScale(Vector3 hitPoint)
        {
            var distance = Vector3.Distance(transform.position, hitPoint);
            _laser.material.SetTextureScale(MainTex, new Vector2(distance, 1));
            _laser.material.SetTextureScale(Noise, new Vector2(distance, 1));
        }

        private void UpdatePosition(Vector3 hitPoint)
        {
            _laser.SetPosition(0, transform.position);
            _laser.SetPosition(1, hitPoint);

            _hitEffect.transform.position = hitPoint;
            _hitEffect.transform.LookAt(hitPoint);
        }

        protected override void Destroy()
        {
            StopAllEffects();
            base.Destroy();
        }

        private void StopAllEffects()
        {
            foreach (var ps in _effects)
            {
                ps.Stop();
            }
        }
    }
}