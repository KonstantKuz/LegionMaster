using System;
using DG.Tweening;
using LegionMaster.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.Tutorial.UI
{
    public class TutorialPopup : MonoBehaviour
    {
        [SerializeField]
        private Image _npcImage;
        [SerializeField]
        private TextMeshProLocalization _message;
        [SerializeField]
        private GameObject _messageContainer;
        [SerializeField]
        private Transform _defaultMessagePosition;
        [SerializeField]
        private float _animationDuration;
        [SerializeField] 
        private GameObject _background;

        private Sequence _currentSequence;

        public void Show(string localizationId, bool showNpc = true)
        {
            Show(localizationId, _defaultMessagePosition.position, showNpc);
        }

        public void Show(string localizationId, Vector3 messagePosition, bool showNpc = true)
        {
            CompleteCurrentAnimation();
            Disabled();
            gameObject.SetActive(true);
            if (showNpc) {
                ShowWithNpc(localizationId, messagePosition);
                return;
            }
            ShowMessage(localizationId, messagePosition);
        }

        public void Hide(bool immediate = true)
        {
            CompleteCurrentAnimation();
            if (_npcImage.gameObject.activeSelf && !immediate) {
                PlayHideNpcAnimation(Disabled);
                return;
            }
            Disabled();
        }
        private void ShowWithNpc(string localizationId, Vector3 messagePosition)
        {
            PlayShowNpcAnimation(() => ShowMessage(localizationId, messagePosition));
        }

        private void PlayShowNpcAnimation(Action endCallback)
        {
            _background.SetActive(true);
            var rectNpc = _npcImage.GetComponent<RectTransform>();
            rectNpc.pivot = Vector2.one;
            _npcImage.color = new Color(_npcImage.color.r, _npcImage.color.g, _npcImage.color.b, 0);
            _npcImage.gameObject.SetActive(true);
            _currentSequence = DOTween.Sequence();
            _currentSequence.Join(rectNpc.DOPivot(Vector2.zero, _animationDuration));
            _currentSequence.Join(_npcImage.DOFade(1, _animationDuration));
            _currentSequence.AppendCallback(() => {
                _currentSequence = null;
                endCallback?.Invoke();
            });
        }

        private void ShowMessage(string localizationId, Vector3 position)
        {
            _messageContainer.transform.position = position;
            _message.LocalizationId = localizationId;
            _messageContainer.gameObject.SetActive(true);
        }

        private void Disabled()
        {
            _npcImage.gameObject.SetActive(false);
            _messageContainer.gameObject.SetActive(false);
            gameObject.SetActive(false);
            _background.SetActive(false);
        }

        private void PlayHideNpcAnimation(Action endCallback)
        {
            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(_npcImage.DOFade(0, _animationDuration));
            _currentSequence.AppendCallback(() => {
                _currentSequence = null;
                endCallback?.Invoke();
            });
        }

        private void CompleteCurrentAnimation()
        {
            if (_currentSequence == null) {
                return;
            }
            _currentSequence.Complete(true);
            _currentSequence = null;
        }
    }
}