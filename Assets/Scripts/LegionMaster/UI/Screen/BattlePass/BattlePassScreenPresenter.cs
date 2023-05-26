using System;
using EasyButtons;
using JetBrains.Annotations;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Config;
using LegionMaster.BattlePass.Service;
using LegionMaster.Extension;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Reward.Model;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.LootBox;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.BattlePass.Model;
using LegionMaster.UI.Screen.BattlePass.View;
using LegionMaster.UI.Screen.Main;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UI.Screen.ProgressUnit;
using LegionMaster.UpgradeUnit.Service;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.BattlePass
{
    public class BattlePassScreenPresenter : BaseScreen
    {
        public const ScreenId BATTLE_PASS_SCREEN = ScreenId.BattlePass; 
        
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + BATTLE_PASS_SCREEN;
        public override ScreenId ScreenId => BATTLE_PASS_SCREEN; 
        public override string Url => URL;

        [SerializeField] private BattlePassExpView _expView;
        [SerializeField] private BattlePassLevelListView _levelListView;     
        [SerializeField] private BattlePassPremiumView _premiumView; 
        [SerializeField] private ActionButton _closeButton;
        [SerializeField] private RectTransform _viewRect;
        [SerializeField] private float _defaultViewOffsetMin;
                
        [Inject] private BattlePassService _battlePassService;
        [Inject] private BattlePassConfigList _battlePassConfigList;      
        [Inject] private ShopService _shop;      
        [Inject] private WalletService _walletService;

        [Inject] private Analytics.Analytics _analytics;
        [Inject] private DialogManager _dialogManager;
        [Inject] private LootBoxOpeningService _lootBoxOpeningService;
        [Inject] private IMessenger _messenger;      
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private UpgradeUnitService _upgradeUnitService;

        public BattlePassScreenModel Model { get; private set; }
        
        private CompositeDisposable _disposable;
        
        [CanBeNull]
        private string _nextScreenUrl;
        

        [PublicAPI]
        public void Init(bool fromMenu, [CanBeNull] string nextScreenUrl)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _nextScreenUrl = nextScreenUrl;
            
            Model = new BattlePassScreenModel(_battlePassService, _battlePassConfigList, _shop, fromMenu, OnTakeReward, OnBuyExp, OnBuyPremium);
          
            _levelListView.Init(Model.LevelListModel);
            _expView.Init(Model.ExpProgress);
            _premiumView.Init(Model.PremiumModel);
            _closeButton.Init(OnClose);
            SetScreenViewOffset(Model.SetFullScreenView);
            
            _battlePassService.Level.Subscribe(level => {
                Model.UpdateLevelModel(level); 
                Model.UpdateLevelModel(level - 1); 
                Model.UpdateExpModel();
            }).AddTo(_disposable);
            _battlePassService.Exp.Subscribe(it => Model.UpdateExpModel()).AddTo(_disposable);
            _walletService.AnyMoneyObservable.Subscribe(it => { Model.UpdateExpModel(); Model.UpdatePremiumModel();}).AddTo(_disposable);
        }
        
        private void SetScreenViewOffset(bool setFullScreenView)
        {
            _viewRect.offsetMin = setFullScreenView ? new Vector2(_viewRect.offsetMin.x, 0) : new Vector2(_viewRect.offsetMin.x, _defaultViewOffsetMin);
        }
        private void OnBuyPremium(string productId)
        {
            using (_analytics.SetAcquisitionProperties(BATTLE_PASS_SCREEN.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                var shopItem = _shop.TryBuy(productId) ?? throw new Exception("Can't buy PremiumBattlePass, shopItem is null");
                _battlePassService.UpdatePremium(true);
                _analytics.BattlePassPremiumBought(_battlePassService.Level.Value);
            }
            Init(Model.FromMenu, _nextScreenUrl);
        }
        private void OnBuyExp()
        {
            using (_analytics.SetAcquisitionProperties(BATTLE_PASS_SCREEN.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                var shopItem = _shop.TryBuyBattlePassExp() ?? throw new Exception("Can't buy BattlePassExp, shopItem is null");
                _battlePassService.AddExp(shopItem.Count, true);
            }
        }
        
        private void OnTakeReward(TakenRewardModel takenRewardModel)
        {
            RewardItem reward;
            using (_analytics.SetAcquisitionProperties(BATTLE_PASS_SCREEN.AnalyticsId(), ResourceAcquisitionType.Continuity)) {
                reward = _battlePassService.TakeReward(takenRewardModel.Id);
                Model.AddTakenReward(reward);
                _analytics.BattlePassRewardGained(takenRewardModel.Id);
            }
            Model.UpdateLevelModel(takenRewardModel.Id.Level);
            CheckAndShowLootBoxDialog(reward);
            reward.TryPublishReceivedLoot(_messenger, takenRewardModel.DroppingLootPosition);
        }

        private void CheckAndShowLootBoxDialog(RewardItem reward)
        {
            if (reward.RewardType != RewardType.LootBox) return;

            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Continuity)) {
                var lootBoxModel = LootBoxModel.FromReward(reward);
                var lootBoxContent = _lootBoxOpeningService.Open(lootBoxModel);
                lootBoxContent.ForEach(it => Model.AddTakenReward(it));
                var lootBoxInitModel = LootBoxDialogInitModel.Common(lootBoxModel, lootBoxContent);
                LootBoxDialogPresenter.ShowWithOpeningAnimation(_dialogManager, lootBoxInitModel);
            }
        }
        private void OnClose()
        {
            if (Model.FromMenu) {
                _screenSwitcher.SwitchTo(MainScreenPresenter.URL);
                return;
            }
            var shardsReward = Model.FindTakenShardReward();
            if (CanUpgradeUnit(shardsReward)) {
                _screenSwitcher.SwitchTo(ProgressUnitPresenter.URL, false, shardsReward.RewardId, _nextScreenUrl);
            } 
            else {
                _screenSwitcher.SwitchTo(_nextScreenUrl);
            }
        }
        private bool CanUpgradeUnit([CanBeNull] RewardItem shardsReward)
        {
            return shardsReward != null && _upgradeUnitService.CanUpgradeNextLevel(shardsReward.RewardId);
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
            Model = null;
        }
        [Button("Add BattlePassExp", Mode = ButtonMode.EnabledInPlayMode)]
        public void AddExp(int amount)
        {
            using (_analytics.SetAcquisitionProperties("UnityEditor.AddBattlePassExp", ResourceAcquisitionType.Boost)) {
                _battlePassService.AddExp(amount);
            }
        }
    }
}