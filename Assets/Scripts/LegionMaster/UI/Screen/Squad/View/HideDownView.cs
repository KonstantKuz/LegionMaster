using DG.Tweening;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.View
{
    public class HideDownView : MonoBehaviour
    {
        [SerializeField] private float _bottomAnchor;

        private Sequence _currentSequence;

        private void OnDisable()
        {
            CompleteCurrentAnimation();
        }

        public Sequence ShowSmoothly(float duration)
        {
            CompleteCurrentAnimation();
            
            var rectTransform = transform as RectTransform;
            var anchorMax = rectTransform.anchorMax;
            var anchorMin = rectTransform.anchorMin;
            
            var moveDistance = anchorMax.y - _bottomAnchor;
            rectTransform.anchorMax = new Vector2(anchorMax.x, anchorMax.y - moveDistance);        
            rectTransform.anchorMin = new Vector2(anchorMin.x, anchorMin.y - moveDistance);
            gameObject.SetActive(true);

            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(rectTransform.DOAnchorMax(new Vector2(anchorMax.x, anchorMax.y), duration));
            _currentSequence.Join(rectTransform.DOAnchorMin(new Vector2(anchorMin.x, anchorMin.y), duration));
            _currentSequence.AppendCallback(() =>
            {
                _currentSequence = null;
            });
            
            return _currentSequence;
        }

        private void CompleteCurrentAnimation()
        {
            if (_currentSequence == null) return;
            _currentSequence.Complete(true);
            _currentSequence = null;
        }

        public Sequence HideSmoothly(float duration)
        {
            CompleteCurrentAnimation();
            
            var rectTransform = transform as RectTransform;
            var anchorMax = rectTransform.anchorMax;
            var anchorMin = rectTransform.anchorMin;

            var moveDistance = anchorMax.y - _bottomAnchor;
            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(rectTransform.DOAnchorMax(new Vector2(anchorMax.x, anchorMax.y - moveDistance), duration));
            _currentSequence.Join(rectTransform.DOAnchorMin(new Vector2(anchorMin.x, anchorMin.y - moveDistance),
                duration));
            _currentSequence.AppendCallback(() =>
            {
                gameObject.SetActive(false);
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                _currentSequence = null;
            });

            return _currentSequence;
        }

    }
}