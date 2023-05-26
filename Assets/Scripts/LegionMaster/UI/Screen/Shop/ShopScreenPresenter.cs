using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.PurchaseInfo;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UI.Screen.Shop.Model;
using LegionMaster.UI.Screen.Shop.View;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Shop
{
    public class ShopScreenPresenter : BaseScreen
    {
        private const ScreenId SHOP_SCREEN = ScreenId.Shop;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + SHOP_SCREEN;

        public override ScreenId ScreenId => SHOP_SCREEN;
        public override string Url => URL;

        [SerializeField]
        private ShopScreenView _shopScreenView;
      
        [Inject] private DialogManager _dialogManager;
        [Inject] private ShopScreenConfig _shopScreenConfig;
        [Inject] private StringKeyedConfigCollection<ProductConfig> _shopConfig;
        [Inject] private StringKeyedConfigCollection<PurchaseConfig> _purchases;
        [Inject] private PackConfigCollection _packs;
        [Inject] private ShopService _shop;
        [Inject] private InAppPurchaseService _inAppPurchaseService;

        private ShopScreenModel _shopScreenModel;

        [PublicAPI]
        public void Init(ShopSectionId selectedSectionId)
        {
            _shopScreenModel = new ShopScreenModel(_shopScreenConfig, _shopConfig, _purchases, _packs, _shop, 
                                                   _inAppPurchaseService, OnItemClick, selectedSectionId);
            _shopScreenView.Init(_shopScreenModel);
        }

        private void OnDisable()
        {
            _shopScreenModel = null;
        }

        private void OnItemClick(string productId)
        {
            _dialogManager.Show<PurchaseInfoDialogPresenter, string>(productId);
        }
    }
}