using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.BattlePass.Config;
using LegionMaster.BattlePass.Model;
using LegionMaster.BattlePass.Service;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Reward.Model;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.Util;
using UniRx;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class BattlePassScreenModel
    {
        private const string PREMIUM_PRODUCT_ID = "BattlePassPremium";

        private readonly List<RewardItem> _takenRewards = new List<RewardItem>();  
        public readonly BattlePassLevelListModel LevelListModel;      
        public readonly bool FromMenu;
        
        private BattlePassService _battlePassService;
        private BattlePassConfigList _battlePassConfigList;
        private ShopService _shop;
        
        private Action<TakenRewardModel> _takeRewardAction;
        private Action _buyExpAction;
        private Action<string> _buyPremiumAction;
    
        private ReactiveProperty<BattlePassExpModel> _expProgress;
        private ReactiveProperty<BattlePassPremiumModel> _premiumModel;   
        
        public IObservable<BattlePassExpModel> ExpProgress => _expProgress;
        public IObservable<BattlePassPremiumModel> PremiumModel => _premiumModel;

        public bool SetFullScreenView => !FromMenu;

        public BattlePassScreenModel(BattlePassService battlePassService, BattlePassConfigList battlePassConfigList, ShopService shop, bool fromMenu,
                                     Action<TakenRewardModel> takeRewardAction, Action buyExpAction, Action<string> buyPremiumAction)
        {
            _battlePassService = battlePassService;
            _battlePassConfigList = battlePassConfigList;
            _shop = shop;
            _takeRewardAction = takeRewardAction;
            _buyExpAction = buyExpAction;
            _buyPremiumAction = buyPremiumAction;
            
            FromMenu = fromMenu;
            LevelListModel = BuildLevelListModel();
            _expProgress = new ReactiveProperty<BattlePassExpModel>(BuildExpModel());
            _premiumModel = new ReactiveProperty<BattlePassPremiumModel>(BuildPremiumModel());
        }

        public void UpdateLevelModel(int level)
        {
            if (level < _battlePassConfigList.GetMinLevelConfig().Level) {
                return;
            }
            var levelModel = BuildLevel(_battlePassConfigList.GetConfigByLevel(level), _battlePassService.PremiumActive.Value);
            LevelListModel.UpdateLevel(levelModel);
        }
        public RewardItem FindTakenShardReward()
        {
            return _takenRewards.LastOrDefault(it => it.RewardType == RewardType.Shards);
        }
        public void AddTakenReward(RewardItem reward) => _takenRewards.Add(reward);

        public void UpdateExpModel()
        { 
            _expProgress.Value = BuildExpModel();
        }  
        public void UpdatePremiumModel()
        { 
            _premiumModel.Value = BuildPremiumModel();
        }
        private BattlePassPremiumModel BuildPremiumModel()
        {
            return new BattlePassPremiumModel() {
                    CanBuyPremium = !_battlePassService.PremiumActive.Value,
                    PriceButton = PriceButtonModel.FromProduct(_shop.GetProductById(PREMIUM_PRODUCT_ID), _shop),
                    BuyPremiumAction = () => _buyPremiumAction?.Invoke(PREMIUM_PRODUCT_ID),
            };
        }
        private BattlePassExpModel BuildExpModel()
        {
            var currentExp = _battlePassService.Exp.Value;         
            var maxExp = _battlePassService.MaxExpForCurrentLevel;
            return new BattlePassExpModel() {
                    ExpProgress = ExpProgressModel.Create(currentExp, maxExp, _battlePassService.Level.Value),
                    PriceButton = BuildPriceButtonExpModel(currentExp, maxExp),
                    BuyExpAction = _buyExpAction,
            };
        }
        private PriceButtonModel BuildPriceButtonExpModel(int currentExp, int maxExp)
        {
            var expPrice = _shop.GetBattlePassExpPrice();
            return new PriceButtonModel() {
                    Price = expPrice,
                    PriceLabel = expPrice.ToString(),
                    Enabled = !(_battlePassService.IsMaxLevel && currentExp >= maxExp),
                    CanBuy = new BoolReactiveProperty(_shop.HasEnoughCurrencyForBattlePassExp()),
                    CurrencyIconPath = IconPath.GetCurrency(Currency.Hard.ToString())
            };
        }
        private BattlePassLevelListModel BuildLevelListModel()
        {
            var premiumActive = _battlePassService.PremiumActive.Value;
            var levels = _battlePassConfigList.Items.Select(it => BuildLevel(it, premiumActive));
            return new BattlePassLevelListModel(levels);
        }
        private BattlePassLevelModel BuildLevel(BattlePassConfig levelConfig, bool premiumActive)
        {
            return new BattlePassLevelModel {
                    LevelInfo = BuildLevelInfoModel(levelConfig),
                    BasicReward = new BattlePassRewardModel(_battlePassService.BuildReward(levelConfig, BattlePassRewardType.Basic), premiumActive),
                    PremiumReward = new BattlePassRewardModel(_battlePassService.BuildReward(levelConfig, BattlePassRewardType.Premium), premiumActive),
                    TakeRewardAction = _takeRewardAction.Invoke,
            };
        }
        private LevelInfoModel BuildLevelInfoModel(BattlePassConfig levelConfig)
        {
            return new LevelInfoModel() {
                    Level = levelConfig.Level,
                    Available = levelConfig.Level <= _battlePassService.Level.Value,
            };
        }
    }
}