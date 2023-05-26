using System;
using LegionMaster.Notification.Provider;
using UniRx;

namespace LegionMaster.Extension
{
    public static class NotificationExtension
    {
        public static IObservable<bool> Exists(this INotification notification) => notification.NotificationCount.Select(it => it > 0);
    }
}