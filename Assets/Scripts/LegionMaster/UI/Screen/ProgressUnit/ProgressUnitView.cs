using System;
using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.ProgressUnit
{
    public class ProgressUnitView : MonoBehaviour
    {
        [SerializeField]
        private Image _unitImage;
        [SerializeField]
        private Image _fragmentImage;
        [SerializeField]
        private ActionButton _upgradeButton;
        [SerializeField]
        private ActionButton _closeButton;
        [SerializeField]
        private ProgressBarView _progressBarView;
        [SerializeField]
        private TMP_Text _fragmentsProgressLabel;

        public void Init(ProgressUnitScreenModel model, Action onUpgrade, Action onClose)
        {
            _upgradeButton.Init(onUpgrade);
            _closeButton.Init(onClose);
            
            UpgradeButtonEnabled = model.UpgradeAvailable;
            SetUnitImage(model);
            SetFragmentsProgress(model);
        }

        private void SetUnitImage(ProgressUnitScreenModel model)
        {
            _unitImage.sprite = Resources.Load<Sprite>(model.UnitIconPath);
            _fragmentImage.sprite = Resources.Load<Sprite>(model.FragmentIconPath);
        }

        private void SetFragmentsProgress(ProgressUnitScreenModel model)
        {
            _progressBarView.SetData(model.FragmentsProgress);
            FragmentsProgressLabel = model.FragmentsProgressLabel;
        }
        private bool UpgradeButtonEnabled
        {
            set
            {
                _upgradeButton.gameObject.SetActive(value);
                _closeButton.gameObject.SetActive(!value);
            }
        }
        private string FragmentsProgressLabel
        {
            set => _fragmentsProgressLabel.text = value;
        }
        private void OnDisable()
        {
            _progressBarView.SetData(0);
        }
    }
}