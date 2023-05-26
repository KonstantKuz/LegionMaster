using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.LootBox.Model;
using LegionMaster.Reward.Model;
using UnityEngine.Assertions;

namespace LegionMaster.UI.Dialog.LootBox.Model
{
    public class LootBoxDialogInitModel
    {
        private readonly List<RewardItem> _rewards;
        
        [CanBeNull] public LootBoxModel LootBox { get; }
        [CanBeNull] public string ShopLootBoxId { get; }
        public ICollection<RewardItem> Rewards => _rewards;
        
        public string Caption { get; }     
        public bool CanBuyLootBox => ShopLootBoxId != null;
        
        private LootBoxDialogInitModel(LootBoxModel lootBox, [CanBeNull] string shopLootBoxId, IEnumerable<RewardItem> rewards, [CanBeNull] string caption = null)
        {
            Assert.IsTrue(shopLootBoxId == null || lootBox != null, "No lootbox is set, but we can buy it");
            LootBox = lootBox;
            ShopLootBoxId = shopLootBoxId;
            _rewards = rewards.ToList();
            Caption = caption;
        }

        public static LootBoxDialogInitModel Common(LootBoxModel lootBox, IEnumerable<RewardItem> lootBoxContent, [CanBeNull] string caption = null)
        {
            return new LootBoxDialogInitModel(lootBox, null, lootBoxContent, caption);
        }     
        public static LootBoxDialogInitModel Debriefing(LootBoxModel lootBox, IEnumerable<RewardItem> lootBoxContent)
        {
            return new LootBoxDialogInitModel(lootBox, null, lootBoxContent);
        }  
        public static LootBoxDialogInitModel Shop(LootBoxModel lootBox, string shopLootBoxId, IEnumerable<RewardItem> lootBoxContent)
        {
            return new LootBoxDialogInitModel(lootBox, shopLootBoxId, lootBoxContent);
        }

        public static LootBoxDialogInitModel ListOfRewards(IEnumerable<RewardItem> rewards, [CanBeNull] string caption = null)
        {
            return new LootBoxDialogInitModel(null, null, rewards, caption);
        }

    }
}