using DG.Tweening;
using UnityEngine;

namespace LegionMaster.UI.Components
{
    public class ScaleBlinking : MonoBehaviour
    {
        [SerializeField] private float _period;
        [SerializeField] private float _maxScaleOffset;

        private void OnEnable()
        {
            transform.DOPunchScale(Vector3.one * _maxScaleOffset, _period, 1).
                SetEase(Ease.Linear).
                SetLoops(-1);
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
        }
    }
}
