using System;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Quest.Message;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using SuperMaxim.Messaging;
using UniRx;
using Zenject;

namespace LegionMaster.Notification.Provider
{
    public class QuestNotification : INotification
    {
        private readonly ReactiveProperty<int> _notificationCount;
        
        [Inject] private QuestService _service;
        
        public IObservable<int> NotificationCount => _notificationCount;

        private QuestNotification(QuestService service, IMessenger messenger)
        {
            _service = service;
            _notificationCount = new ReactiveProperty<int>(GetNotificationCount());
            messenger.Subscribe<QuestStateChangedMessage>(_ => UpdateNotification());
            messenger.Subscribe<QuestSectionRewardTakenMessage>(_ => UpdateNotification());
        }
        public bool HasNotificationForSection(QuestSectionType type)
        {
            return HasCompletedQuestsInSection(type) || _service.GetSection(type).HasUntakenRewards();
        }
        private bool HasCompletedQuestsInSection(QuestSectionType type)
        {
            return _service.GetQuestBySection(type).Any(it => it.Completed && !it.RewardTaken);
        }  
        private int GetNotificationCount() =>
                EnumExt.Values<QuestSectionType>()
                       .Sum(NotificationCountForSection);
        private int NotificationCountForSection(QuestSectionType type)
        {
            return CompletedQuestsCountInSection(type) + _service.GetSection(type).UntakenRewardCount();
        }
        private int CompletedQuestsCountInSection(QuestSectionType type)
        {
            return _service.GetQuestBySection(type).Count(it => it.Completed && !it.RewardTaken);
        }
        private void UpdateNotification() => _notificationCount.Value = GetNotificationCount();
        
        
          
    }
}