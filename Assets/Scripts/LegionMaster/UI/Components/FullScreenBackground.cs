using LegionMaster.UI.Screen;
using UnityEngine;

namespace LegionMaster.UI.Components
{
    class FullScreenBackground : MonoBehaviour
    {
        private SafeAreaContainer _safeAreaContainer;
        
        private void OnEnable()
        {
            var fullScreenRect = SafeAreaContainer.transform.parent as RectTransform;
            FitToRectTransform(fullScreenRect);
        }

        private void FitToRectTransform(RectTransform fullScreenRect)
        {
            var rectTransform = transform as RectTransform;
            var corners = new Vector3[4];
            fullScreenRect.GetWorldCorners(corners);
            var bottomLeft = corners[0];
            var topRight = corners[2];
            rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = 0.5f * Vector2.one;
            var parentRectTransform = rectTransform.parent as RectTransform;
            rectTransform.offsetMin = WorldToLocalPoint(parentRectTransform, bottomLeft);;
            rectTransform.offsetMax = WorldToLocalPoint(parentRectTransform, topRight);;
        }

        private static Vector2 WorldToLocalPoint(RectTransform rectTransform, Vector3 worldPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                RectTransformUtility.WorldToScreenPoint(null, worldPos), null, out var rez);
            return rez;
        }

        private SafeAreaContainer SafeAreaContainer => _safeAreaContainer ??= GetComponentInParent<SafeAreaContainer>();
    }
}