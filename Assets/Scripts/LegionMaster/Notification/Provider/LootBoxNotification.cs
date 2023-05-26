using System;
using LegionMaster.Extension;
using LegionMaster.LootBox.Message;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Repository;
using LegionMaster.Shop.Service;
using SuperMaxim.Messaging;
using UniRx;

namespace LegionMaster.Notification.Provider
{
    public class LootBoxNotification : INotification
    {
        private const string LOOT_BOX_NOTIFICATION_PRODUCT = "LootBoxRareSmall";

        private readonly ShopService _shop;
        private readonly LootBoxNotificationRepository _repository;
        private readonly ReactiveProperty<int> _notificationCount;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        public IObservable<int> NotificationCount => _notificationCount;

        public LootBoxNotification(ShopService shop,
                                   LootBoxNotificationRepository repository,
                                   WalletService walletService,
                                   IMessenger messenger)
        {
            _shop = shop;
            _repository = repository;

            if (NotificationState.Shown) {
                _notificationCount = new ReactiveProperty<int>(0);
                return;
            }
            _notificationCount = new ReactiveProperty<int>(NoticeCount);
            messenger.MessageAsObservable<LootBoxShopOpenMessage>(OnLootBoxShopOpened).AddTo(_disposable);
            walletService.AnyMoneyObservable.Subscribe(it => UpdateNotificationProperty(NoticeCount)).AddTo(_disposable);
        }

        private void OnLootBoxShopOpened(LootBoxShopOpenMessage msg)
        {
            if (NoticeCount == 0) {
                return;
            }
            Unsubscribe();
            var state = NotificationState;
            state.Shown = true;
            _repository.Set(state);
            UpdateNotificationProperty(0);
        }

        private void Unsubscribe()
        {
            _disposable.Dispose();
        }

        private void UpdateNotificationProperty(int count) => _notificationCount.Value = count;

        private int NoticeCount => _shop.HasEnoughCurrency(LOOT_BOX_NOTIFICATION_PRODUCT) ? 1 : 0;
        private LootBoxNotificationState NotificationState => _repository.Get() ?? new LootBoxNotificationState();
    }
}