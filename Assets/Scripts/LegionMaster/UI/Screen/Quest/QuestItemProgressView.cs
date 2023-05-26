using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.Quest
{
    public class QuestItemProgressView : MonoBehaviour
    {
        private const string GET_REWARD_LOCALIZATION_ID = "GetReward";
        private const string COMPLETED_LOCALIZATION_ID = "Completed";
        
        [SerializeField] private GameObject _progressBarContainer;
        [SerializeField] private ProgressBarView _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private GameObject _getRewardBackground;
        [SerializeField] private TextMeshProLocalization _stateText;
        [SerializeField] private Color _getRewardTextColor;
        [SerializeField] private Color _completedTextColor;

        public void Init(QuestItemModel model)
        {
            UpdateProgress(model);
            UpdateViewByState(model.State);
        }

        private void UpdateProgress(QuestItemModel model)
        {
            _progressText.text = $"{model.Count} / {model.MaxCount}";
            _progressBar.SetValueWithLoop((float) model.Count / model.MaxCount);
        }

        private void UpdateViewByState(QuestItemModel.QuestState questState)
        {
            SwitchTextToBar();

            switch (questState)
            {
                case QuestItemModel.QuestState.Completed:
                    SetGetRewardState();
                    break;
                case QuestItemModel.QuestState.RewardTaken:
                    SetRewardTakenState();
                    break;
            }
        }

        private void SwitchTextToBar()
        {
            _progressBarContainer.SetActive(true);
            _stateText.gameObject.SetActive(false);
        }

        private void SetGetRewardState()
        {
            SwitchBarToText();
            _getRewardBackground.SetActive(true);
            _stateText.LocalizationId = GET_REWARD_LOCALIZATION_ID;
            _stateText.TextComponent.color = _getRewardTextColor;
        }

        private void SetRewardTakenState()
        {
            SwitchBarToText();
            _stateText.LocalizationId = COMPLETED_LOCALIZATION_ID;
            _stateText.TextComponent.color = _completedTextColor;
        }

        private void SwitchBarToText()
        {
            _progressBarContainer.SetActive(false);
            _stateText.gameObject.SetActive(true);
        }
    }
}