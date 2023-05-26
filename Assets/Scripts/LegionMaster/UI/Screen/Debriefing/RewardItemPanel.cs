using JetBrains.Annotations;
using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class RewardItemPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProLocalization _rewardLabel;
        [SerializeField]
        private TMP_Text _rewardCount;
        [SerializeField]
        private Image _rewardIcon;

        [SerializeField]
        [CanBeNull]
        private Image _iconBackground;
        [SerializeField]
        [CanBeNull]
        private Image _unitIcon;

        public void Init(RewardItemModel model)
        {
            RewardLabel = model.RewardLabel;
            RewardCount = model.RewardCount;

            LoadAndSetIcon(model.IconPath, model.IsUnitShards);
            TryLoadAndSetBackground(model);
        }

        private void TryLoadAndSetBackground(RewardItemModel model)
        {
            if (_iconBackground == null) {
                return;
            }
            _iconBackground.gameObject.SetActive(model.BackgroundPath != null);
            if (model.BackgroundPath != null) {
                _iconBackground.sprite = Resources.Load<Sprite>(model.BackgroundPath);
            }
        }

        private void LoadAndSetIcon(string iconPath, bool isUnitShards)
        {
            var sprite = Resources.Load<Sprite>(iconPath);

            if (isUnitShards) {
                _unitIcon.sprite = sprite;
            } else {
                _rewardIcon.sprite = sprite;
            }
            _unitIcon?.gameObject.SetActive(isUnitShards);
            _rewardIcon.gameObject.SetActive(!isUnitShards);
        }

        private string RewardCount
        {
            set => _rewardCount.text = value;
        }
        private string RewardLabel
        {
            set => _rewardLabel.LocalizationId = value;
        }
    }
}