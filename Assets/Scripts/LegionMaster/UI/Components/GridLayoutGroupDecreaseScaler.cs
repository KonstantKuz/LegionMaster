using DG.Tweening;
using UnityEngine;

namespace LegionMaster.UI.Components
{
    public class GridLayoutGroupDecreaseScaler : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _scaleStep = 0.2f;
        [SerializeField] private int _itemsAmount = 4;  
        [SerializeField] private float _scaleDuration = 0.5f;
        
        private Vector3 _initScale;

        private void Awake()
        {
            var localScale = _rect.localScale;
            _initScale = new Vector3(localScale.x + _scaleStep * ScaleStepCount, localScale.y + _scaleStep * ScaleStepCount, _initScale.z);
        }
        public void InitScale()
        {
            _rect.localScale = _initScale;
        }
        
        public void DecreaseScale(int itemNumber)
        {
            if (itemNumber <= 1 || itemNumber > _itemsAmount) {
                return;
            }
            var previousScale = new Vector3(_initScale.x - _scaleStep * ScaleStepNumber(itemNumber - 1),
                                            _initScale.y - _scaleStep * ScaleStepNumber(itemNumber - 1), _initScale.z);
            _rect.localScale = previousScale;
            var endScale = new Vector3(_initScale.x - _scaleStep * ScaleStepNumber(itemNumber), _initScale.y - _scaleStep * ScaleStepNumber(itemNumber), _initScale.z);
            _rect.DOScale(endScale, _scaleDuration);
        }
        private float ScaleStepNumber(int itemNumber) => itemNumber - 1;
        private float ScaleStepCount => _itemsAmount - 1;
        
    }
}