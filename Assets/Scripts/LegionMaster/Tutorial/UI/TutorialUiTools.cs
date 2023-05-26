using System.Collections;
using LegionMaster.Tutorial.WaitConditions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegionMaster.Tutorial.UI
{
    public class TutorialUiTools : MonoBehaviour
    {
        [SerializeField]
        private TutorialHand _tutorialHand;
        [SerializeField]
        private TutorialPopup _tutorialPopup;
        [SerializeField]
        private UIElementHighlighter _elementHighlighter;
        [SerializeField]
        private UiRectHighlighter rectHighlighter;
        [SerializeField]
        private GameObject _overlay;

        public TutorialHand TutorialHand => _tutorialHand;

        public TutorialPopup TutorialPopup => _tutorialPopup;

        public UIElementHighlighter ElementHighlighter => _elementHighlighter;

        public UiRectHighlighter RectHighlighter => rectHighlighter;
        
        public void ShowOverlay() => _overlay.gameObject.SetActive(true);

        public void HideOverlay() => _overlay.gameObject.SetActive(false);

        public IEnumerator WaitForClick() => new WaitForClick(_overlay.GetComponent<UIBehaviour>());
    }
}