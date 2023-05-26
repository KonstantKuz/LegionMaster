using JetBrains.Annotations;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.Extension;
using LegionMaster.Factions.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.Footnote.Model;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.Description.View;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UI.Screen.Squad.Faction;
using LegionMaster.UI.Screen.Squad.Faction.View;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using LegionMaster.UpgradeUnit.Service;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Description
{
    public class DescriptionUnitPresenter : BaseScreen
    {
        private const ScreenId DESCRIPTION_SCREEN = ScreenId.UnitDescription;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + DESCRIPTION_SCREEN;
        public override ScreenId ScreenId => DESCRIPTION_SCREEN; 
        public override string Url => URL;
        
        [SerializeField] private StatisticsView _statisticsView; // todo will be used
        [SerializeField] private DescriptionView _descriptionView;
        [SerializeField] private FactionView _factionView;
        [SerializeField] private ButtonWithProgress _craftButton;
        [SerializeField] private ButtonWithProgress _levelUpButton;
        
        [Inject] private UnitModelBuilder _unitModelBuilder;       
        [Inject] private InventoryService _inventoryService;      
        [Inject] private UpgradeUnitService _upgradeUnitService;
        [Inject] private WalletService _walletService;       
        [Inject] private PlayerProgressService _playerProgressService;
        [Inject] private Analytics.Analytics _analytics;    
        [Inject] private UnitUpgradableStateProvider _unitUpgradableStateProvider;
        [Inject] private DialogManager _dialogManager;
        [Inject] private FactionConfigCollection _factionConfig;
        
        private CompositeDisposable _disposable;
        
        private DescriptionUnitScreenModel _model;

        [PublicAPI]
        public void Init(string unitId)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            _model = new DescriptionUnitScreenModel(unitId, _inventoryService, _unitModelBuilder, _unitUpgradableStateProvider, ShowFactionFootnote);
            _descriptionView.Init(_model.UnitDescription);
            _factionView.Init(_model.Factions);
                    
            _model.ProgressButtonCraftModel.Subscribe(it => _craftButton.Init(it, OnCraftClicked)).AddTo(_disposable); 
            _model.ProgressButtonLevelModel.Subscribe(it => _levelUpButton.Init(it, OnLevelUpClicked)).AddTo(_disposable);

            _walletService.GetMoneyAsObservable(Currency.Soft).Subscribe(it => _model.RebuildPriceLevelModel()).AddTo(_disposable);
            _playerProgressService.LevelAsObservable.Subscribe(it => _model.RebuildPriceLevelModel()).AddTo(_disposable);
        }
        
        private void OnCraftClicked()
        {
            var unitId = _model.UnitDescription.Value.UnitId;
            using (_analytics.SetAcquisitionProperties(DESCRIPTION_SCREEN.AnalyticsId(), ResourceAcquisitionType.Boost))
            { 
                _inventoryService.TryUnlockUnit(unitId);
            }
            _model.RebuildAll();
        } 
        private void OnLevelUpClicked()
        {
            using (_analytics.SetAcquisitionProperties(DESCRIPTION_SCREEN.AnalyticsId(), ResourceAcquisitionType.Boost))
            {
                _upgradeUnitService.UpgradeNextLevel(_model.UnitDescription.Value.UnitId);
                _model.RebuildAll();                
            }
        } 
        private void ShowFactionFootnote(string factionId, RectTransform position)
        {
            FactionPresenter.ShowFactionFootnote(factionId, position, Side.RightTop, _factionConfig, _dialogManager);
        }
        private void OnDisable()
        {
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}