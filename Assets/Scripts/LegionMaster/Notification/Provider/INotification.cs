using System;

namespace LegionMaster.Notification.Provider
{
    public interface INotification
    {
        IObservable<int> NotificationCount { get; }
    }
}