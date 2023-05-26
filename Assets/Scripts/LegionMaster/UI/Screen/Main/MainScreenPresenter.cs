using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Session.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.CampaignSquad;
using LegionMaster.UI.Screen.Footer;
using LegionMaster.UI.Screen.LootBoxShop;
using LegionMaster.UI.Screen.Menu;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Main
{
    public class MainScreenPresenter : BaseScreen
    {
        public const ScreenId MAIN_SCREEN = ScreenId.Main;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + MAIN_SCREEN;
        public override ScreenId ScreenId => MAIN_SCREEN;
        public override string Url => URL;

        [SerializeField] private ActionButton _toBattleButton;
        [SerializeField] private ActionButton _lootboxButton;
        [SerializeField] private NoUnitPopupPresenter _noUnitsPopup;
        
        [Inject] private ScreenSwitcher _screenSwitcher;

        [Inject] private LocationArena _locationArena;
        [Inject] private UnitGroupView _unitGroupView;
        [Inject] private InventoryService _inventoryService;
        [Inject] private SessionBuilder _sessionBuilder;     
        [Inject] private ShopShowChecker _shopShowChecker;
           
        [PublicAPI]
        public void Init()
        {
            _toBattleButton.Init(StartCampaign);
            _lootboxButton.Init(ShowLootboxScreen);
            _unitGroupView.Init(_inventoryService.UnlockedUnitIds.ToList());
            _shopShowChecker.TryShowPackOffer();
        }
        private void OnEnable()
        {
            _locationArena.HideScene();
        }

        private void OnDisable()
        {
            _noUnitsPopup.Hide();
            if (_locationArena)
            {
                _locationArena.ShowScene();
            }
            if (_unitGroupView)
            {
                _unitGroupView.Hide();
            }
        }

        private void StartCampaign()
        {
            _sessionBuilder.CreateCampaign();
            _screenSwitcher.SwitchTo(CampaignSquadPresenter.URL);
        }

        private void ShowLootboxScreen()
        {
            _screenSwitcher.SwitchTo(LootBoxShopPresenter.URL);
        }
    }
}