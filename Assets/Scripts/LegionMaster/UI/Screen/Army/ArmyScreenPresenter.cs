using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Description;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UI.Screen.Squad.View;
using LegionMaster.Units.Config;
using LegionMaster.UpgradeUnit.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Army
{
    public class ArmyScreenPresenter : BaseScreen
    {
        private const ScreenId ARMY_SCREEN = ScreenId.Army;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + ARMY_SCREEN;
        public override ScreenId ScreenId => ARMY_SCREEN; 
        public override string Url => URL;

        [SerializeField] private UnitButtonsView _unitButtonsView;
        
        [Inject] private InventoryService _inventoryService;
        [Inject] private UnitUpgradableStateProvider _unitUpgradableStateProvider;
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private ScreenSwitcher _screenSwitcher;
        
        private ArmyScreenModel _model;

        private void OnEnable()
        {
            _model = new ArmyScreenModel(_unitCollectionConfig.AllUnitIds, _inventoryService, _unitUpgradableStateProvider, OnUnitClicked);
            _unitButtonsView.Init(_model.UnitButtons);
        }

        private void OnDisable()
        {
            _model = null;
        }

        private void OnUnitClicked(string unitId)
        {
            _screenSwitcher.SwitchTo(DescriptionUnitPresenter.URL, false, unitId);
        }
    }
}