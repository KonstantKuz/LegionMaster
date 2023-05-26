using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Notification.Provider;
using Zenject;

namespace LegionMaster.Notification
{
    public class NotificationService
    {
        private IDictionary<NotificationType, INotification> _providers;
        
        public NotificationService(DiContainer container)
        {
            _providers = EnumExt.Values<NotificationType>()
                                .ToDictionary(it => it, it => (INotification) 
                                                      container.Instantiate(it.GetProvider()));

        }
        public INotification Get(NotificationType type) => _providers[type];
    }
}