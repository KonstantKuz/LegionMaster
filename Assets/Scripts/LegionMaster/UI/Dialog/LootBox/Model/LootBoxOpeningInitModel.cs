using System;
using LegionMaster.LootBox.Model;

namespace LegionMaster.UI.Dialog.LootBox.Model
{
    public class LootBoxOpeningInitModel
    {
        public LootBoxId LootBoxId { get; }
        public Action OnAnimationComplete { get; set; }

        public LootBoxOpeningInitModel(LootBoxId lootBoxId, Action onAnimationComplete)
        {
            LootBoxId = lootBoxId;
            OnAnimationComplete = onAnimationComplete;
        }
    }
}