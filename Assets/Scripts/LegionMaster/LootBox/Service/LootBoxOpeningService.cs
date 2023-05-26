using System;
using System.Collections.Generic;
using LegionMaster.LootBox.Message;
using LegionMaster.LootBox.Model;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.LootBox.Service
{
    public class LootBoxOpeningService
    {
        [Inject]
        private CommonRewardService _commonRewardService;
        [Inject]
        private IRewardApplyService _rewardApplyService;
        [Inject]
        private IMessenger _messenger;
        public List<RewardItem> Open(LootBoxModel lootBox)
        {
            if (lootBox.Count < 1) {
                throw new ArgumentException($"LootBoxModel count cannot be < 1, lootBoxId:= {lootBox.Id}");
            }
            var takenRewards = new List<RewardItem>();
            for (int i = 0; i < lootBox.Count; i++) {
                takenRewards.Add(_commonRewardService.CalculateReward(lootBox.Id.ToString()));
            }
            _rewardApplyService.ApplyRewards(takenRewards);
            _messenger.Publish(new LootboxOpenMessage
            {
                    Count = lootBox.Count
            });
            return takenRewards;
        }
    }
}