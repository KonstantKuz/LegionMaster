using System;
using LegionMaster.UI.Dialog.Footnote.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegionMaster.UI.Dialog.Footnote.View
{
    public class FootnoteView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private FootnoteDescriptionView _footnoteDescriptionView;
      
        private Action _onClose;
        public void Init(FootnoteInitModel initModel, Action onClose)
        {
            _onClose = onClose;
            _footnoteDescriptionView.Init(initModel);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClose?.Invoke();
        }
    }
}