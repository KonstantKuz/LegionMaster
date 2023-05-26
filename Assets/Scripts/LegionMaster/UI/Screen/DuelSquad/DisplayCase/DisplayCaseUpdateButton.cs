using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.DuelSquad.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.DisplayCase
{
    public class DisplayCaseUpdateButton : MonoBehaviour
    {
        private static readonly int PressedHash = Animator.StringToHash("Pressed");  
        private static readonly int AvailableHash = Animator.StringToHash("Available");
        
        private ClickableObject _updateButton;
        private Animator _animator;
        private CompositeDisposable _disposable;
        
        public void Init(DisplayCaseUpdateButtonModel model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            UpdateButton.Init((item) => OnClick(model.OnClick));
            model.Available.Subscribe(UpdateAvailable).AddTo(_disposable);
        }

        private void UpdateAvailable(bool available)
        {
            _updateButton.enabled = available;
            Animator.SetBool(AvailableHash, available);
        }
        private void OnClick(Action onClick)
        {
            onClick?.Invoke();
            Animator.SetTrigger(PressedHash);
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        private ClickableObject UpdateButton => _updateButton ??= GetComponent<ClickableObject>();
        private Animator Animator => _animator ??= GetComponent<Animator>();
    }
}