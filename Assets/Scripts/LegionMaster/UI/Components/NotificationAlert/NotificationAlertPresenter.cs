using LegionMaster.Extension;
using LegionMaster.Notification;
using LegionMaster.Notification.Provider;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Components.NotificationAlert
{
    public class NotificationAlertPresenter : MonoBehaviour
    {
        [SerializeField]
        private NotificationType _type;
        [SerializeField] private bool _showCount;
        
        [SerializeField] private ActivatableObject _notificationAlert;
        [SerializeField] private TextView _notificationCount;
        
        
        [Inject] private NotificationService _notificationService;

        private void OnEnable()
        {
            _notificationAlert.Init(Notification.Exists());
            _notificationCount.gameObject.SetActive(_showCount);
            if (_showCount) {
                _notificationCount.Init(Notification.NotificationCount);
            }
        }
        private INotification Notification => _notificationService.Get(_type);
    }
}