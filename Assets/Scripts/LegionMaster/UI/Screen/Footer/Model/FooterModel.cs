using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Notification;
using LegionMaster.Player.Inventory.Message;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Army;
using LegionMaster.UI.Screen.DuelSquad;
using LegionMaster.UI.Screen.Main;
using LegionMaster.UI.Screen.Quest;
using LegionMaster.UI.Screen.Shop;
using LegionMaster.UI.Screen.Shop.Model;
using LegionMaster.UpgradeUnit.Message;
using LegionMaster.UpgradeUnit.Service;
using SuperMaxim.Core.Extensions;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine.Assertions;

namespace LegionMaster.UI.Screen.Footer.Model
{
    public class FooterModel
    {
        public struct ScreenSwitchParams
        {
            public string Url;
            public bool SwitchImmediately;
            public object[] Params;
        }
        private static readonly Dictionary<FooterButtonType, ScreenSwitchParams> ButtonToUrlMap = new Dictionary<FooterButtonType, ScreenSwitchParams>
        {
                {FooterButtonType.Quest, new ScreenSwitchParams
                {
                        Url = QuestScreenPresenter.URL, 
                        SwitchImmediately = true,
                }},
                {FooterButtonType.Shop, new ScreenSwitchParams
                {
                        Url = ShopScreenPresenter.URL, 
                        SwitchImmediately = true,
                        Params = new object[] { ShopSectionId.SpecialOffers },
                }},
                {FooterButtonType.Campaign, new ScreenSwitchParams
                {
                        Url = MainScreenPresenter.URL, 
                        SwitchImmediately = false,
                }},
                {FooterButtonType.Army, new ScreenSwitchParams
                {
                        Url = ArmyScreenPresenter.URL, 
                        SwitchImmediately = true,
                }},
                {FooterButtonType.Duel, new ScreenSwitchParams
                {
                        Url = DuelSquadPresenter.URL, 
                        SwitchImmediately = true,
                        Params = new object[] {true},
                }}
        };
        
        private readonly InventoryService _inventoryService;
        private readonly UnitUpgradableStateProvider _upgradableStateProvider;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly ReactiveProperty<FooterButtonType> _selectedButton; 
        private readonly Dictionary<FooterButtonType, BoolReactiveProperty> _notifications;

        private readonly Action<FooterButtonType> _onClickAction;

     

        public FooterModel(Action<FooterButtonType> onButtonClicked, 
                           NotificationService notificationService, 
                           InventoryService inventoryService, 
                           WalletService walletService, 
                           UnitUpgradableStateProvider upgradableStateProvider, 
                           IMessenger messenger, 
                           string screenName)
        {
            _inventoryService = inventoryService;
            _upgradableStateProvider = upgradableStateProvider;
            _selectedButton = new ReactiveProperty<FooterButtonType>(GetButtonByScreenName(screenName));

            _notifications = EnumExt.Values<FooterButtonType>()
                                    .ToDictionary(it => it, it => new BoolReactiveProperty());
            
            SetupNotifications(notificationService, FooterButtonType.Quest);
            SetupArmyNotifications(walletService, messenger);
            
            _onClickAction = onButtonClicked;
        }

        public static FooterButtonType GetButtonByScreenName(string screenName)
        {
            foreach (var pair in ButtonToUrlMap)
            {
                if (pair.Value.Url.EndsWith(screenName)) return pair.Key;
            }

            return FooterButtonType.Campaign;
        }

        private void SetupNotifications(NotificationService notificationService, params FooterButtonType[] types)
        {
            types.ForEach(buttonType => {
                notificationService.Get(buttonType.GetNotificationType()).Exists()
                                   .Subscribe(it => _notifications[buttonType].Value = it).AddTo(_disposable);
            });
          
        }
        private void SetupArmyNotifications(WalletService walletService, IMessenger messenger)
        {
            UpdateArmyNotification();
            walletService.AnyMoneyObservable.Subscribe(it => UpdateArmyNotification()).AddTo(_disposable);
            messenger.MessageAsObservable<InventoryChangedMessage>(_ => UpdateArmyNotification()).AddTo(_disposable);
            messenger.MessageAsObservable<UnitUpgradeMessage>(_ => UpdateArmyNotification()).AddTo(_disposable);
        }
        public void Term()
        {
            _disposable.Dispose();
        }
        private void UpdateArmyNotification()
        {
            _notifications[FooterButtonType.Army].Value = _inventoryService.PlayerUnits.Any(unit => _upgradableStateProvider.Get(unit).HaveAnyUpgrade);
        }

        public void SetSelected(FooterButtonType buttonType)
        {
            _selectedButton.SetValueAndForceNotify(buttonType);
        }

        public FooterButtonModel GetButtonModel(FooterButtonType type) =>
            new FooterButtonModel
            {
                Notification = _notifications[type],
                OnClick = _onClickAction,
                SelectedButton = _selectedButton,
                Type = type
            };

        public static ScreenSwitchParams GetScreenSwitchParams(FooterButtonType buttonType)
        {
            Assert.IsTrue(ButtonToUrlMap.ContainsKey(buttonType), $"No url set for button {buttonType}");
            return ButtonToUrlMap[buttonType];
        }
    }
}