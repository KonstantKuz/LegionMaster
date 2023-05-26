using UnityEngine;

namespace LegionMaster.Extension
{
    public static class RectTransformExtension
    {
        public static void CopyTargetRect(this RectTransform origin, RectTransform target)
        {
            origin.pivot = target.pivot;
            origin.position = target.position;
            origin.localScale = target.localScale;
            origin.anchoredPosition = target.anchoredPosition;
            origin.anchorMin = target.anchorMin;
            origin.anchorMax = target.anchorMax;
            origin.offsetMin = target.offsetMin;
            origin.offsetMax = target.offsetMax;
            origin.rect.Set(target.rect.x, target.rect.y, target.rect.width, target.rect.height);
        }
    }
}