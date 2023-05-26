using System;
using System.Collections;
using DG.Tweening;
using LegionMaster.UIMessage.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegionMaster.UIMessage
{
    public class UIMessage : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        private const float PANEL_START_POSITION = 0f;
        private const float PANEL_START_PIVOT_Y = 0;
        private const float PANEL_END_PIVOT_Y = 1;
        private const float PANEL_SLIDE_TIME = 0.7f;
        public int Timeout { get; private set; }
        public DateTime ExpirationTime { get; private set; }
        public UnityAction HideCallback { get; private set; }
        public MessageType MessageType { get; private set; }

        private RectTransform _rectTransform;
        private Graphic[] _graphics;
        
        public void Init(int timeout, DateTime expirationTime, MessageType messageType, UnityAction hideCallback = null)
        {
            Timeout = timeout;
            ExpirationTime = expirationTime;
            HideCallback = hideCallback;
            MessageType = messageType;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            RectTransform.anchoredPosition = new Vector2(PANEL_START_POSITION, PANEL_START_POSITION);
            RectTransform.pivot = new Vector2(RectTransform.pivot.x, PANEL_START_PIVOT_Y);
        }

        public void Show()
        {
            DoFadeGraphics(PANEL_SLIDE_TIME, 1);
            if (MessageType == MessageType.TOP) {
                PlaySlideDown();
            }
        }
        private void PlaySlideDown()
        {
            RectTransform.DOPivotY(PANEL_END_PIVOT_Y, PANEL_SLIDE_TIME).SetUpdate(true);
        }

        public void Hide()
        {
            DoFadeGraphics(PANEL_SLIDE_TIME, 0);
            if (MessageType == MessageType.TOP) {
                StartCoroutine(PlaySlideUp());
            }
        }
        private IEnumerator PlaySlideUp()
        {
            yield return RectTransform.DOPivotY(PANEL_START_PIVOT_Y, PANEL_SLIDE_TIME).WaitForCompletion();
            Destroy(gameObject);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            HideCallback?.Invoke();
        }
        public void OnDrag(PointerEventData eventData)
        {
            HideCallback?.Invoke();
        }
        private void DoFadeGraphics(float fadeTime, float targetAlpha)
        {
            foreach (Graphic graphic in Graphics) {
                if (Math.Abs(fadeTime) <= float.Epsilon) {
                    Color color = graphic.color;
                    color = new Color(color.r, color.g, color.b, targetAlpha);
                    graphic.color = color;
                    continue;
                }
                graphic.DOFade(targetAlpha, fadeTime);
            }
        }
        public bool Expired
        {
            get { return ExpirationTime <= DateTime.Now; }
        }
        private RectTransform RectTransform => _rectTransform ??= gameObject.GetComponent<RectTransform>();
        private Graphic[] Graphics => _graphics ??= GetComponentsInChildren<Graphic>();
    }
}