using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.Extension
{
    public static class ScrollRectExtensions
    {
        public static void ScrollToChild(this ScrollRect scrollRect, RectTransform child)
        {
            scrollRect.content.localPosition = GetScrollPositionByChild(scrollRect, child);
        }

        public static IEnumerator ScrollToChildCoroutine(this ScrollRect scrollRect, RectTransform child, float duration)
        {
            var newScrollPosition = GetScrollPositionByChild(scrollRect, child);
            if (IsScrollPositionOnChild(scrollRect.content.localPosition, newScrollPosition)) {
                yield break;
            }
            var verticalEnabled = scrollRect.vertical;  
            var horizontalEnabled = scrollRect.horizontal;
            SetActiveScroll(scrollRect, false, false);
            yield return scrollRect.content.DOLocalMove(newScrollPosition, duration).WaitForCompletion();
            SetActiveScroll(scrollRect, verticalEnabled, horizontalEnabled);
        }

        private static void SetActiveScroll(ScrollRect scrollRect, bool verticalEnabled, bool horizontalEnabled)
        {
            scrollRect.vertical = verticalEnabled;
            scrollRect.horizontal = horizontalEnabled;
        }
        private static Vector2 GetScrollPositionByChild(ScrollRect scrollRect, RectTransform child)
        {
            Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            return Vector2.zero - (viewportLocalPosition + childLocalPosition);
        }

        private static bool IsScrollPositionOnChild(Vector2 scrollPosition, Vector2 newScrollPosition) =>
                Mathf.Abs(scrollPosition.y - newScrollPosition.y) < float.Epsilon
                && Mathf.Abs(scrollPosition.x - newScrollPosition.x) < float.Epsilon;
    }
}