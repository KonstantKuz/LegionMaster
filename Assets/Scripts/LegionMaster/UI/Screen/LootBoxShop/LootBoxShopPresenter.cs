using System;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.Config;
using LegionMaster.Extension;
using LegionMaster.LootBox.Config;
using LegionMaster.LootBox.Message;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.LootBox;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UIMessage.Model;
using LegionMaster.UIMessage.Panel;
using LegionMaster.UIMessage.Service;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class LootBoxShopPresenter : BaseScreen
    {
        private const string NOT_ENOUGH_CURRENCY_MESSAGE = "There is not enough currency to buy a product";
        private const ScreenId LOOT_BOX_SCREEN = ScreenId.LootBox;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + LOOT_BOX_SCREEN;
        public override ScreenId ScreenId => LOOT_BOX_SCREEN; 
        public override string Url => URL;

        [SerializeField] private LootBoxShopView _lootBoxShopView;
        
        [Inject] private LootBoxShopConfig _lootBoxShopConfig;     
        [Inject] private StringKeyedConfigCollection<ProductConfig> _shopConfig;
        [Inject] private ShopService _shop;
        [Inject] private DialogManager _dialogManager;
        [Inject] private Analytics.Analytics _analytics;
        [Inject] private WalletService _walletService;     
        [Inject] private UIMessagePresenter _uiMessagePresenter;
        [Inject] private LootBoxOpeningService _lootBoxOpeningService;
        [Inject] private IMessenger _messenger;      
        [Inject] private PackConfigCollection _packs;
        
        private CompositeDisposable _disposable;
        private LootBoxShopScreenModel _model;

        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _model = new LootBoxShopScreenModel(_shopConfig, _lootBoxShopConfig, _shop, _packs, OnBuyLootBox);
            _lootBoxShopView.Init(_model);
            _walletService.GetMoneyAsObservable(Currency.Soft).Subscribe(it => _model.UpdateProducts()).AddTo(_disposable);     
            _walletService.GetMoneyAsObservable(Currency.Hard).Subscribe(it => _model.UpdateProducts()).AddTo(_disposable);
            _messenger.Publish(new LootBoxShopOpenMessage());
        }

        private void OnBuyLootBox(string productId)
        {
            if (!_shop.HasEnoughCurrency(productId)) {
                ShowUIMessage();
                return;
            }
            LootBoxModel lootBox;
            using (_analytics.SetAcquisitionProperties(LOOT_BOX_SCREEN.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                lootBox = _shop.TryBuyLootBox(productId) ?? throw new Exception($"Can't buy loot box, id= {productId}");
            }
            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Boost)) {
                var dialogInitModel = LootBoxDialogInitModel.Shop(lootBox, productId, _lootBoxOpeningService.Open(lootBox));
                LootBoxDialogPresenter.ShowWithOpeningAnimation(_dialogManager, dialogInitModel);
            }
        }
        private void ShowUIMessage()
        {
            var message = CommonMessageModel.Create(NOT_ENOUGH_CURRENCY_MESSAGE).Timeout(1).Build();
            _uiMessagePresenter.ShowCommon<CommonUIMessagePanel>(message);
        }
        private void OnDisable()
        {
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}