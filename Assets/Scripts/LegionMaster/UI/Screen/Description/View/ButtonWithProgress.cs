using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Description.Model;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.Description.View
{
    public class ButtonWithProgress : MonoBehaviour
    {
        [SerializeField] private ActionButton _actionButton;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private ProgressBarView _progressBar;

        public void Init(ProgressButtonModel model, Action action)
        {
            _actionButton.Init(action);

            Enabled = model.Enabled;
            SetProgressLabel($"{model.CurrentValue}/{model.MaxValue}");
            SetInteractableState(model.Interactable);
            
            _progressBar.Reset();
            _progressBar.SetValueWithLoop(model.Progress);
        }

        private void SetProgressLabel(string value)
        {
            _progressLabel.text = value;
        }

        private void SetInteractableState(bool value)
        {
            _actionButton.Button.interactable = value;
        }

        private bool Enabled
        {
            set => gameObject.SetActive(value);
        }
    }
}
