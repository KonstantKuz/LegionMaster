using JetBrains.Annotations;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.Description;
using LegionMaster.UpgradeUnit.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.ProgressUnit
{
    public class ProgressUnitPresenter : BaseScreen
    {
        private const ScreenId PROGRESS_UNIT_SCREEN = ScreenId.ProgressUnit;
        public static readonly string URL = BattleScreen.BATTLE_SCREEN + "/" + PROGRESS_UNIT_SCREEN;
        public override ScreenId ScreenId => PROGRESS_UNIT_SCREEN; 
        public override string Url => URL;

      
        [SerializeField] private ProgressUnitView _progressUnitView;
        
        [Inject] private InventoryService _inventoryService;
        [Inject] private UpgradeUnitService _upgradeUnitService;
        [Inject] private ScreenSwitcher _screenSwitcher;

        private ProgressUnitScreenModel _model;
        private string _nextScreenUrl;
        
        [PublicAPI] 
        public void Init(string unitId, string nextScreenUrl)
        {
            _model = new ProgressUnitScreenModel(unitId, _inventoryService, _upgradeUnitService);
            _progressUnitView.Init(_model, OnUpgrade, OnClose);
            _nextScreenUrl = nextScreenUrl;
        }
        private void OnDisable()
        {
            _model = null;
        }

        private void OnClose()
        {
            _screenSwitcher.SwitchTo(_nextScreenUrl, true);
        }

        private void OnUpgrade()
        { 
            _screenSwitcher.SwitchTo(DescriptionUnitPresenter.URL, false, _model.UnitId);
        }
    }
}