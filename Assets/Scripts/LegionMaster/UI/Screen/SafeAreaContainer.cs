using UnityEngine;

namespace LegionMaster.UI.Screen
{
    public class SafeAreaContainer : MonoBehaviour
    {
        private void Awake()
        {
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = UnityEngine.Screen.safeArea;
            /*
            Ignore bottom safe area - nothing important is here 
            rectTransform.anchorMin = new Vector2(
                safeArea.xMin / UnityEngine.Screen.width,
                safeArea.yMin / UnityEngine.Screen.height);
            */
            rectTransform.anchorMax = new Vector2(
                safeArea.xMax / UnityEngine.Screen.width,
                safeArea.yMax / UnityEngine.Screen.height);
        }
    }
}