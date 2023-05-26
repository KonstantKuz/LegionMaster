using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Notification.Provider;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Quest
{
    public class QuestScreenModel
    {
        public readonly List<QuestItemModel> Quests;
        public readonly SectionButtonModel DailyButton;
        public readonly SectionButtonModel WeeklyButton;
        public readonly QuestSectionType SectionType;
        public readonly SectionProgressModel SectionProgress;

        public QuestScreenModel(QuestService questService, QuestSectionType sectionType,
                                QuestNotification notification,
                                Action<QuestSectionType> switchSectionAction, 
            Action<string, Transform> takeRewardAction,
            Action<int, Transform> takeSectionRewardAction)
        {
            Quests = BuildQuests(questService.GetQuestBySection(sectionType), takeRewardAction);
            SectionType = sectionType;
            
            DailyButton = BuildButton(QuestSectionType.Daily, switchSectionAction, notification);
            WeeklyButton = BuildButton(QuestSectionType.Weekly, switchSectionAction, notification);

            SectionProgress = BuildSectionProgress(questService.GetSection(sectionType), takeSectionRewardAction);
        }
        
        private SectionButtonModel BuildButton(QuestSectionType type, Action<QuestSectionType> switchSectionAction,
                                               QuestNotification notification)
        {
            return new SectionButtonModel
            {
                Caption = type == QuestSectionType.Daily ? "Daily Quests" : "Weekly Quests",
                EndTime = LegionMaster.Quest.Model.Quest.GetEndOfCurrentPeriod(type),
                Action = () => switchSectionAction?.Invoke(type),
                IsSelected = type == SectionType,
                HasNotification = new ReactiveProperty<bool>(notification.HasNotificationForSection(type))
            };
        }

        private static List<QuestItemModel> BuildQuests(IEnumerable<LegionMaster.Quest.Model.Quest> quests, Action<string, Transform> takeRewardAction)
        {
            return quests.Select(it => new QuestItemModel
            {
                LocalizationId = $"{it.Condition}QuestTask",
                Count = it.Counter,
                MaxCount = it.Config.ConditionCount,
                ExpReward = it.Config.Points,
                Reward = it.Config.Reward.ToRewardItem(),
                State = GetQuestState(it),
                ClickAction = (position) => takeRewardAction.Invoke(it.Id, position)
            }).ToList();
        }

        private static QuestItemModel.QuestState GetQuestState(LegionMaster.Quest.Model.Quest quest)
        {
            if (quest.RewardTaken) return QuestItemModel.QuestState.RewardTaken;
            if (quest.Completed) return QuestItemModel.QuestState.Completed;
            return QuestItemModel.QuestState.Active;
        }
        
        private SectionProgressModel BuildSectionProgress(QuestSection questSection, Action<int, Transform> takeSectionReward)
        {
            return new SectionProgressModel
            {
                Exp = questSection.Points,
                Progress = 1.0f * questSection.Points / questSection.MaxPoints,
                Rewards = questSection.Rewards.Select(it => new SectionProgressModel.RewardItemModel
                {
                    Config = it.Config,
                    State = questSection.GetRewardState(it),
                    ClickAction = (position) => takeSectionReward.Invoke(it.Config.RequiredPoints, position)
                }).ToArray() 
            };
        }
    }
}