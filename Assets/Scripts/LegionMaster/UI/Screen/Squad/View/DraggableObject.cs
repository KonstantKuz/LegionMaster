using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegionMaster.UI.Screen.Squad.View
{
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Action<GameObject> _mouseDown;
        private Action<GameObject> _mouseUp;

        public void Init(Action<GameObject> mouseDown, Action<GameObject> mouseUp)
        {
            _mouseDown = mouseDown;
            _mouseUp = mouseUp;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _mouseDown?.Invoke(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _mouseUp?.Invoke(gameObject);
        }
    }
}