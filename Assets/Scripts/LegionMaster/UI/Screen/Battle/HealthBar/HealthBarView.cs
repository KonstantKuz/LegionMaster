using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Battle.HealthBar
{
    public class HealthBarView : UnitBarView
    {
        [SerializeField] private Image _hitEffectImage;
        [SerializeField] private float _hitEffectTime = 0.1f;

        private Color _initColor;
        private Sequence _hitVfxSequence;

        public override void Init(UnitBarModel model)
        {
            _initColor = _hitEffectImage.color;            
            base.Init(model);
        }

        protected override void SetValue(float value)
        {
            bool playHitEffect = IsHealthDecreased(value);
            base.SetValue(value);
            if (playHitEffect) {
                PlayHitEffect(Color.white);
            }
        }

        private bool IsHealthDecreased(float healthValue)
        {
            return healthValue < _bar.Value;
        }

        private void PlayHitEffect(Color effectColor)
        {
            CancelHitVfx();
            _hitEffectImage.color = _initColor;
            _hitVfxSequence = DOTween.Sequence()
                .Append(_hitEffectImage.DOColor(effectColor, _hitEffectTime))
                .Append(_hitEffectImage.DOColor(_initColor, _hitEffectTime));
        }

        protected override void OnDisable()
        {
            CancelHitVfx();
            base.OnDisable();
        }

        private void CancelHitVfx()
        {
            if (_hitVfxSequence == null) return;
            _hitVfxSequence.Kill();
            _hitVfxSequence = null;
        }
    }
}