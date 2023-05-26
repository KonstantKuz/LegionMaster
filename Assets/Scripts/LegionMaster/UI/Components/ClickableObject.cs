using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegionMaster.UI.Components
{
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        private Action<GameObject> _mouseClick;

        public void Init(Action<GameObject> mouseClick)
        {
            _mouseClick = mouseClick;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _mouseClick?.Invoke(gameObject);
        }
    }
}