using LegionMaster.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.Tutorial.UI
{
    public class UiRectHighlighter : MonoBehaviour
    {
        [SerializeField] private UIElementHighlighter _elementHighlighter;

        [SerializeField] private Image _cutoutMask;
        [SerializeField] private GameObject _cutoutBackground;

        public void Set(Component component) => Set(component.gameObject);

        public void Set(GameObject uiElement)
        {
            _elementHighlighter.Set(uiElement);
            _elementHighlighter.Background.SetActive(false);

            var uiElementTransform = uiElement.GetComponent<RectTransform>();
            _cutoutMask.rectTransform.CopyTargetRect(uiElementTransform);
            _cutoutBackground.transform.SetParent(_cutoutMask.transform);
            
            _cutoutMask.gameObject.SetActive(true);
            _cutoutBackground.SetActive(true);
        }
        
        public void Clear()
        {
            _elementHighlighter.Clear();
            
            _cutoutBackground.transform.SetParent(null);
            _cutoutBackground.gameObject.SetActive(false);
            _cutoutMask.gameObject.SetActive(false);
        }
    }
}