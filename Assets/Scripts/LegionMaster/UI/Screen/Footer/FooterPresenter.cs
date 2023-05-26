using LegionMaster.Location.Session.Service;
using LegionMaster.Notification;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Footer.Model;
using LegionMaster.UpgradeUnit.Service;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Footer
{
    public class FooterPresenter : MonoBehaviour
    {
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private InventoryService _inventoryService;
        [Inject] private UnitUpgradableStateProvider _upgradableStateProvider;
        [Inject] private WalletService _walletService;
        [Inject] private IMessenger _messenger;
        [Inject] private NotificationService _notificationService;       
        [Inject] private SessionBuilder _sessionBuilder;    
        
        private FooterModel _model;

        private void Init(string screenName)
        {
            _model = new FooterModel(OnButtonClicked,
                                     _notificationService, 
                                     _inventoryService, 
                                     _walletService, 
                                     _upgradableStateProvider,
                                     _messenger,
                                     screenName);

            foreach (var button in GetComponentsInChildren<FooterButtonView>())
            {
                button.Init(_model.GetButtonModel(button.Type));
            }
        }

        private void OnDisable()
        {
            _model?.Term();
            _model = null;
        }

        private void OnButtonClicked(FooterButtonType buttonType)
        {
            if (buttonType == FooterButtonType.Duel) {
                _sessionBuilder.CreateDuel();
            }
            var switchParams = FooterModel.GetScreenSwitchParams(buttonType);
            _screenSwitcher.SwitchTo(switchParams.Url, !switchParams.SwitchImmediately, switchParams.Params ?? new object[] {});
        }

        public void SetCurrentScreen(string screenName)
        {
            if (_model == null)
            {
                Init(screenName);
            }
            else
            {
                _model.SetSelected(FooterModel.GetButtonByScreenName(screenName));
            }
        }
    }
}