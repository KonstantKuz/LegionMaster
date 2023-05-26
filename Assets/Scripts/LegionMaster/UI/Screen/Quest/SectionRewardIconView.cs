using System;
using LegionMaster.Quest.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Quest
{
    [RequireComponent(typeof(Button))]
    public class SectionRewardIconView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _points;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardCount;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _completedColor;
        [SerializeField] private Color _rewardTakenColor;
        [SerializeField] private GameObject _rewardTakenCheckmark;
        [SerializeField] private Image _pointer;
        private Button _button;
        private Action _clickAction;

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }

        public void Init(SectionProgressModel.RewardItemModel model)
        {
            _points.text = model.Config.RequiredPoints.ToString();
            _rewardIcon.sprite = Resources.Load<Sprite>(Util.IconPath.GetCurrency(model.Config.Reward.ToRewardItem().RewardId));
            _rewardCount.text = model.Config.Reward.Count.ToString();
            _pointer.color = GetColor(model.State);
            _rewardTakenCheckmark.SetActive(model.State == QuestSection.RewardState.RewardTaken);
            _clickAction = () => model.ClickAction.Invoke(DroppingLootPosition);
            Button.interactable = model.State == QuestSection.RewardState.Completed;
        }
        private Color GetColor(QuestSection.RewardState modelState)
        {
            return modelState switch
            {
                QuestSection.RewardState.Active => _defaultColor,
                QuestSection.RewardState.Completed => _completedColor,
                QuestSection.RewardState.RewardTaken => _rewardTakenColor,
                _ => throw new ArgumentOutOfRangeException(nameof(modelState), modelState, null)
            };
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
            _clickAction?.Invoke();
        }
    }
}