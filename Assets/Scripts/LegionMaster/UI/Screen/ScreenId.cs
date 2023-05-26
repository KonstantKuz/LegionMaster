using System.Collections.Generic;

namespace LegionMaster.UI.Screen
{
    public enum ScreenId
    {
        None,
        Menu,
        Main,
        Quest,
        Battle,
        Squad,
        LootBox,
        ProgressUnit,
        Duel,
        DuelSquad,
        DuelDebriefing,
        UnitDescription,
        Debriefing,      
        BattlePass,
        BattlePlay, 
        BattleCountdown,
        Army,
        BattleMode,
        Shop,
        Campaign,
        CampaignSquad,
        CampaignDebriefing,
        UnitsTransition,
        HyperCasual,
        
    }
    
    public static class ScreenTypeExt
    {
        private static readonly Dictionary<ScreenId, string> AnalyticsIds = new Dictionary<ScreenId, string>() {
                [ScreenId.Main] = "mainScreen",    
                [ScreenId.Quest] = "questScreen",
                [ScreenId.Squad] = "squadScreen", 
                [ScreenId.LootBox] = "chestShopScreen",  
                [ScreenId.ProgressUnit] = "progressUnitScreen", 
                [ScreenId.DuelSquad] = "duelSquadScreen",
                [ScreenId.DuelDebriefing] = "duelDebriefingScreen", 
                [ScreenId.UnitDescription] = "characterScreen",       
                [ScreenId.Debriefing] = "debriefingScreen",      
                [ScreenId.BattlePass] = "battlePassScreen",   
                [ScreenId.BattlePlay] = "battlePlayScreen",     
                [ScreenId.Army] = "armyScreen",   
                [ScreenId.BattleMode] = "battleModeScreen",
                [ScreenId.Shop] = "shopScreen",
                [ScreenId.CampaignSquad] = "campaignSquadScreen", 
                [ScreenId.CampaignDebriefing] = "campaignDebriefingScreen",
                [ScreenId.UnitsTransition] = "unitsTransitionScreen",
                [ScreenId.BattleCountdown] = "battleCountdownScreen",
                [ScreenId.HyperCasual] = "hyperCasualScreen",
        };
        public static bool CanBeInAnalytics(this ScreenId value) => AnalyticsIds.ContainsKey(value);
        public static string AnalyticsId(this ScreenId value) => AnalyticsIds[value];
    }
}