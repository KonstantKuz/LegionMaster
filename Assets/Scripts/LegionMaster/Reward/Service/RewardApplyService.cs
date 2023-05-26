using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Reward.Model;
using Zenject;
using CurrencyExt = LegionMaster.Player.Inventory.Model.CurrencyExt;

namespace LegionMaster.Reward.Service
{
    [PublicAPI]
    public class RewardApplyService : IRewardApplyService
    {
        [Inject]
        private InventoryService _inventoryService;
        [Inject]
        private WalletService _walletService;
        [Inject]
        private PlayerProgressService _progressService;
        [Inject]
        private Analytics.Analytics _analytics;     
        [Inject]
        private BattlePassService _battlePassService;
        public void ApplyReward(RewardItem rewardItem)
        {
            switch (rewardItem.RewardType) {
                case RewardType.Shards:
                    _inventoryService.AddUnitFragments(rewardItem.RewardId, rewardItem.Count);
                    break;
                case RewardType.Currency:
                    _walletService.Add(CurrencyExt.ValueOf(rewardItem.RewardId), rewardItem.Count);
                    break;
                case RewardType.LootBox:
                    _analytics.ReportResourceGained(CurrencyType.Chest, rewardItem.Count);
                    break;   
                case RewardType.Exp:
                    _progressService.AddExp(rewardItem.Count);
                    break;
                case RewardType.BattlePassExp:
                    _battlePassService.AddExp(rewardItem.Count);
                    break;
                default:
                    throw new ArgumentException($"RewardType not found, type:= {rewardItem.RewardType}");
            }
        }
        public void ApplyRewards(IEnumerable<RewardItem> items)
        {
            foreach (var rewardItem in items)
            {
                ApplyReward(rewardItem);
            }
        }
    }
}