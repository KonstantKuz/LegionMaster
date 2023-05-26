using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Reward.Model;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Description.Model;
using SuperMaxim.Core.Extensions;
using UniRx;

namespace LegionMaster.UI.Dialog.LootBox.Model
{
    public class LootBoxDialogModel
    {
        private ReactiveCollection<RewardItemCollection> _rewardCollection;
        private readonly ReactiveProperty<LootBoxButtonModel> _lootBoxButtonModel;
        private readonly ShopService _shop;
        
        public readonly LootBoxDialogInitModel InitModel;
        public readonly Action OnRewardCollectionOpened;
        public ICollection<RewardItem> RewardItems => InitModel.Rewards;
        public IReadOnlyReactiveCollection<RewardItemCollection> RewardCollection => _rewardCollection;
        public IReadOnlyReactiveProperty<LootBoxButtonModel> LootBoxButtonModel => _lootBoxButtonModel;

        public LootBoxDialogModel(LootBoxDialogInitModel initModel, ShopService shop)
        {
            InitModel = initModel;
            _shop = shop;
            _lootBoxButtonModel = new ReactiveProperty<LootBoxButtonModel>(BuildLootBoxButtonModel(false, false));
            OnRewardCollectionOpened = () => UpdateLootBoxButtonModel(true, InitModel.CanBuyLootBox);
            BuildReactiveRewardCollection(RewardItems);
        }

        private void UpdateLootBoxButtonModel(bool closeButtonEnabled, bool buyButtonEnabled)
        {
            _lootBoxButtonModel.Value = BuildLootBoxButtonModel(closeButtonEnabled, buyButtonEnabled);
        }

        private LootBoxButtonModel BuildLootBoxButtonModel(bool closeButtonEnabled, bool buyButtonEnabled)
        {
            return new LootBoxButtonModel() {
                    CloseButtonEnabled = closeButtonEnabled,
                    BuyButtonEnabled = buyButtonEnabled,
                    PriceButton = buyButtonEnabled ? BuildPriceButton() : null,
            };
        }

        private PriceButtonModel BuildPriceButton()
        {
            var product = _shop.GetProductById(InitModel.ShopLootBoxId);
            return PriceButtonModel.FromProduct(product, _shop);
        }

        private void BuildReactiveRewardCollection(IEnumerable<RewardItem> rewardItems)
        {
            _rewardCollection = new ReactiveCollection<RewardItemCollection>();
            var rewardItemCollection = BuildRewardItemCollection(rewardItems.Select(BuildRewardItemModel));
            _rewardCollection.Add(rewardItemCollection);
        }

        private static RewardItemCollection BuildRewardItemCollection(IEnumerable<RewardItemModel> rewardItemModels)
        {
            return new RewardItemCollection(rewardItemModels);
        }

        private static RewardItemModel BuildRewardItemModel(RewardItem rewardItems)
        {
            return RewardItemModel.Create("", "", rewardItems);
        }

        public void AddRewards(ICollection<RewardItem> rewardItems)
        {
            rewardItems.ForEach(it => RewardItems.Add(it));
            UpdateLootBoxButtonModel(true, false);
            _rewardCollection.Add(BuildRewardItemCollection(rewardItems.Select(BuildRewardItemModel)));
        }
    }
}