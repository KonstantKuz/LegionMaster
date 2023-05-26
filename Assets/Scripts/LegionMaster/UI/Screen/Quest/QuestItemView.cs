using System;
using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Quest
{
    [RequireComponent(typeof(Button))]
    public class QuestItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProLocalization _name;
        [SerializeField] private TextMeshProLocalization _expReward;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardCount;
        [SerializeField] private QuestItemProgressView _progressView;
        [SerializeField] private GameObject _completedBackground;
        [SerializeField] private GameObject _rewardTakenBackground;
        private Button _button;
        private Action _action;
        
        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }

        public void Init(QuestItemModel model)
        {
            _name.SetTextFormatted(model.LocalizationId, model.MaxCount);
            _progressView.Init(model);

            _rewardIcon.sprite = Resources.Load<Sprite>(Util.IconPath.GetCurrency(model.Reward.RewardId));
            _rewardCount.text = model.Reward.Count.ToString();
            _expReward.SetTextFormatted("+{0} progress exp", model.ExpReward);
            
            _completedBackground.SetActive(model.State == QuestItemModel.QuestState.Completed);
            _rewardTakenBackground.SetActive(model.State == QuestItemModel.QuestState.RewardTaken);
            
            Button.interactable = model.State == QuestItemModel.QuestState.Completed;
            _action = () => model.ClickAction.Invoke(DroppingLootPosition);
        }
        
        private Transform DroppingLootPosition => _rewardIcon.transform;
        private Button Button
        {
            get
            {
                return _button ??= GetComponent<Button>();
            }
        }
 
        private void OnClick()
        {
            _action?.Invoke();
        }
    }
}