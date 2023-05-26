using System.Collections.Generic;
using LegionMaster.Player.Progress.Model;
using UnityEngine;

namespace LegionMaster.Analytics.Data
{
    public static class AnalyticsCollectibles
    {
        private static readonly Dictionary<PlayerCollectibles, string> AnalyticsValue =
            new Dictionary<PlayerCollectibles, string>()
            {
                [PlayerCollectibles.Battle] = "battle",
                [PlayerCollectibles.Character] = "getCharacter",
                [PlayerCollectibles.QuestDaily] = "getRewardQuestDaily",
                [PlayerCollectibles.QuestWeekly] = "getRewardQuestWeekly",
                [PlayerCollectibles.LootBoxCommon] = "buyChestCommon",
                [PlayerCollectibles.LootBoxRare] = "buyChestRare",
            };
        
        public static string ToAnalytics(PlayerCollectibles collectible)
        {
            if (AnalyticsValue.ContainsKey(collectible))
                return AnalyticsValue[collectible];
            
            Debug.Log($"Error! Dictionary {AnalyticsValue} doesn't contain key {collectible}!");
            return null;
        }        
    }
}