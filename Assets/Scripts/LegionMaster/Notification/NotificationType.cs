using System;
using System.Collections.Generic;
using LegionMaster.Notification.Provider;

namespace LegionMaster.Notification
{
    public enum NotificationType
    {
        BattlePass, 
        Quest,
        LootBox
    }

    public static class NotificationTypeExt
    {
        private static Dictionary<NotificationType, Type> _providers =
                new Dictionary<NotificationType, Type>() {
                        {NotificationType.BattlePass, typeof(BattlePassNotification)},
                        {NotificationType.Quest, typeof(QuestNotification)},     
                        {NotificationType.LootBox, typeof(LootBoxNotification)},
                };

        public static Type GetProvider(this NotificationType value) => _providers[value];
    }
}