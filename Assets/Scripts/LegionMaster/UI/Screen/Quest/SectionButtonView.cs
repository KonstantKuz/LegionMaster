using System;
using LegionMaster.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Quest
{
    [RequireComponent(typeof(Button))]
    public class SectionButtonView : MonoBehaviour
    {
        [SerializeField] private TextMeshProLocalization _caption;
        [SerializeField] private TimeLeftTextView _timeLeft;
        [SerializeField] private ActivatableObject _notification;
        private Button _button;
        private Action _action;

        private Button Button
        {
            get
            {
                return _button ??= GetComponent<Button>();
            }
        }

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }

        public void Init(SectionButtonModel model)
        {
            _caption.LocalizationId = model.Caption;
            _action = model.Action;
            Button.interactable = !model.IsSelected;
            _timeLeft.Init(model.EndTime);
            _notification.Init(model.HasNotification);
        }

        private void OnClick()
        {
            _action?.Invoke();
        }
    }
}