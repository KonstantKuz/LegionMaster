using System.Collections.Generic;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Reward.Model;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.LootBox;
using LegionMaster.UI.Dialog.LootBox.Model;
using Zenject;

namespace LegionMaster.UI.Components
{
    public class RewardPresenter
    {
        [Inject] private DialogManager _dialogManager;
        [Inject] private LootBoxOpeningService _lootBoxOpeningService;
        [Inject] private Analytics.Analytics _analytics;
        
        public void ShowIfNeeded(List<RewardItem> rewardItems)
        {
            foreach (var reward in rewardItems) {
                switch (reward.RewardType) {
                    case RewardType.LootBox:
                        ShowLootBoxDialog(reward);
                        break;
                }
            }
        }
        private void ShowLootBoxDialog(RewardItem reward)
        {
            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Continuity))
            {
                var lootBox = LootBoxModel.FromReward(reward);
                var lootBoxInitModel = LootBoxDialogInitModel.Common(lootBox, _lootBoxOpeningService.Open(lootBox));
                LootBoxDialogPresenter.ShowWithOpeningAnimation(_dialogManager, lootBoxInitModel);
            }
        }
    }
}