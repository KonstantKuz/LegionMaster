using LegionMaster.DroppingLoot.Message;
using LegionMaster.Reward.Model;
using SuperMaxim.Messaging;
using UnityEngine;

namespace LegionMaster.Extension
{
    public static class RewardItemExtension
    { 
        public static void TryPublishReceivedLoot(this RewardItem reward, IMessenger messenger, Vector2 startPosition)
        {
            if (!reward.RewardType.ShouldPlayDropVfx()) {
                return;
            }
            messenger.Publish(UiLootReceivedMessage.FromReward(reward, startPosition));
        }
    }
}