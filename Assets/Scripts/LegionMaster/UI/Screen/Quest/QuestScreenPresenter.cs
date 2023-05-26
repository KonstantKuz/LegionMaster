using System.Linq;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.Extension;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Notification;
using LegionMaster.Notification.Provider;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Model;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.LootBox;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.Menu;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Quest
{
    public class QuestScreenPresenter : BaseScreen
    {
        private const string FORGOTTEN_REWARD_POPUP_CAPTION = "Rewards for completed quests";
        public const ScreenId QUEST_SCREEN = ScreenId.Quest;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + QUEST_SCREEN;
        public override ScreenId ScreenId => QUEST_SCREEN;
        public override string Url => URL;

        [SerializeField] private QuestListView _questListView;
        [SerializeField] private SectionButtonView _dailyButton;
        [SerializeField] private SectionButtonView _weeklyButton;
        [SerializeField] private SectionProgressView _sectionProgressView;

        private QuestScreenModel _model;
        
        [Inject] private QuestService _questService;
        [Inject] private Analytics.Analytics _analytics;
        [Inject] private DialogManager _dialogManager;
        [Inject] private LootBoxOpeningService _lootBoxOpeningService;
        [Inject] private IMessenger _messenger;       
        [Inject] private NotificationService _notificationService;
        private void OnEnable()
        {
            Init(QuestSectionType.Daily);
        }

        private void Init(QuestSectionType sectionType)
        {
            _model = new QuestScreenModel(_questService, sectionType, QuestNotification, SwitchMode, TakeQuestReward, TakeSectionReward);
            _questListView.Init(_model.Quests);
            _dailyButton.Init(_model.DailyButton);
            _weeklyButton.Init(_model.WeeklyButton);
            _sectionProgressView.Init(_model.SectionProgress);

            ShowForgottenRewardsPopupIfAny();
        }

        private void ShowForgottenRewardsPopupIfAny()
        {
            using (_analytics.SetAcquisitionProperties(QUEST_SCREEN.AnalyticsId(), ResourceAcquisitionType.Continuity))
            {
                var forgottenRewards = _questService.TakeOldRewards();
                if (forgottenRewards.Count == 0) return;

                var dialogInitModel = LootBoxDialogInitModel.ListOfRewards(
                    forgottenRewards.Select(it => it.ToRewardItem()), FORGOTTEN_REWARD_POPUP_CAPTION);
                _dialogManager.Show<LootBoxDialogPresenter, LootBoxDialogInitModel>(dialogInitModel);
            }
        }

        private void SwitchMode(QuestSectionType sectionType)
        {
            Init(sectionType);
        }

        private void TakeQuestReward(string questId, Transform rewardPosition)
        {
            using (_analytics.SetAcquisitionProperties(QUEST_SCREEN.AnalyticsId(), ResourceAcquisitionType.Continuity))
            {
                var reward = _questService.TakeQuestReward(questId);
                CheckAndShowLootBoxDialog(reward);
                TryPublishReceivedLoot(reward, rewardPosition);
            }
            UpdateState();
        }

        private void UpdateState()
        {
            Init(_model.SectionType);
        }

        private void TakeSectionReward(int points, Transform rewardPosition)
        {
            RewardItemConfig reward;
            using (_analytics.SetAcquisitionProperties(QUEST_SCREEN.AnalyticsId(), ResourceAcquisitionType.Continuity))
            {
                reward = _questService.TakeSectionReward(new QuestSectionRewardId(_model.SectionType, points));
            }
            CheckAndShowLootBoxDialog(reward);
            TryPublishReceivedLoot(reward, rewardPosition);
            UpdateState();
        }

        private void TryPublishReceivedLoot(RewardItemConfig reward, Transform rewardPosition)
        {
            reward.ToRewardItem().TryPublishReceivedLoot(_messenger, rewardPosition.position);
        }
        private void CheckAndShowLootBoxDialog(RewardItemConfig reward)
        {
            if (reward.Type != RewardType.LootBox) return;

            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Continuity))
            {
                var lootBoxModel = LootBoxModel.FromReward(reward.ToRewardItem());
                var lootBoxInitModel = LootBoxDialogInitModel.Common(lootBoxModel, _lootBoxOpeningService.Open(lootBoxModel));
                LootBoxDialogPresenter.ShowWithOpeningAnimation(_dialogManager, lootBoxInitModel);
            }
        }

        private QuestNotification QuestNotification => (QuestNotification) _notificationService.Get(NotificationType.Quest);
    }
}