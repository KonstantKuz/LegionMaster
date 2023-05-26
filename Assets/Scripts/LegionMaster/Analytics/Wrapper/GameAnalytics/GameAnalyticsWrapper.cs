using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;
using UnityEngine;

namespace LegionMaster.Analytics.Wrapper.GameAnalytics
{
    public class GameAnalyticsWrapper : IAnalyticsImpl
    {
        private const string BATTLE_EVENT = "Battle";
        private const string DUEL_EVENT = "Duel";
        private const string CAMPAIGN_EVENT = "Campaign";      
        private const string HYPER_CASUAL_EVENT = "HC";
        
        private const string VS_BOT_MODE = "vsBot";
        private const string VICTORY = "Win";
        private const string DEFEAT = "Fail";

        public void Init()
        {
            Debug.Log("Initializing GameAnalytics SDK");
            ApplyUserIdCheat();
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }
        
        private static void ApplyUserIdCheat()
        {
            if (!GameAnalyticsId.ShouldGenerateNewId()) return;
            GameAnalyticsSDK.GameAnalytics.SetCustomId(DateTime.Now.Ticks.ToString());
            GameAnalyticsId.ClearFlag();
        }
        public void ReportTest()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("Test");
        }
        public void ReportBattleStart(int battleNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, BATTLE_EVENT, VS_BOT_MODE, battleNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Battle:{battleNum}:Start");
        }

        public void ReportBattleFirstStart(int totalProgress)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Battle:Start:{totalProgress}");
        }

        public void ReportBattleEnd(int battleNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(isPlayerWon ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                                                               BATTLE_EVENT,
                                                               VS_BOT_MODE,
                                                               battleNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Battle:{battleNum}:{BattleResult(isPlayerWon)}");
        }
        
        public void ReportDuelStart(int duelNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, DUEL_EVENT, VS_BOT_MODE, duelNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:Match:{duelNum}:Start");
        }

        public void ReportDuelEnd(int duelNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(
                                                               isPlayerWon ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                                                               DUEL_EVENT,
                                                               VS_BOT_MODE,
                                                               duelNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:Match:{duelNum}:{BattleResult(isPlayerWon)}");
        }

        public void ReportCampaignStart(int campaignNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, CAMPAIGN_EVENT, VS_BOT_MODE, campaignNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Campaign:Chapter:{campaignNum}:Start");
        }

        public void ReportCampaignEnd(int campaignNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(
                isPlayerWon ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                CAMPAIGN_EVENT,
                VS_BOT_MODE,
                campaignNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Campaign:Chapter:{campaignNum}:{BattleResult(isPlayerWon)}");
        }

        public void ReportHyperCasualBattleStart(int battleNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, HYPER_CASUAL_EVENT, VS_BOT_MODE, battleNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"HC:{battleNum}:Start");
        }

        public void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(isPlayerWon ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                                                               HYPER_CASUAL_EVENT,
                                                               VS_BOT_MODE,
                                                               battleNum.ToString());
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"HC:{battleNum}:{BattleResult(isPlayerWon)}");
        }

        public void ReportStartNoUnits()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("startNoUnits");
        }
        public void ReportUnitPlaceChanged()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("unit_place_changed");
        }

        public void ReportResourceGained(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType, string acquisitionPlace)
        {
            GameAnalyticsSDK.GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currencyType.ToString(), amount, acquisitionType.ToString(), acquisitionPlace);
        }

        public void ReportResourceSpent(CurrencyType currencyType, float amount,
            ResourceAcquisitionType acquisitionType, string acquisitionPlace)
        {
            GameAnalyticsSDK.GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, currencyType.ToString(), amount, acquisitionType.ToString(), acquisitionPlace);
        }

        public void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param)
        {
            var eventString = $"character:{unitId}";
            if (eventName != null)
            {
                eventString += $":{eventName}";
            }

            if (param != null)
            {
                foreach (var pair in param)
                {
                    eventString += $":{pair.Key}:{pair.Value}";
                }
            }
            
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventString);
        }

        public void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"quest:sectionid:{questSection}:{questId}:{eventId}");
        }

        public void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"quest:sectionid:{rewardId.Type}:{rewardId.Points}:{eventId}");
        }

        public void ReportProfileEvent(PlayerCollectibles collectible, int num, int delta)
        {
            var eventKey = AnalyticsCollectibles.ToAnalytics(collectible);
            if (eventKey == null)
            {
                return;
            }
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventKey, delta);
        }

        public void PlaceUnitByTap()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("placeUnitByTap");
        }

        public void PlaceUnitByDragRight()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("placeUnitByDragRight");
        }

        public void ReportDuelRoundStart(int duelNum, int roundNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:MatchRound{duelNum}:Round{roundNum}:Start");
        }

        public void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:MatchRound:{duelNum}:Round{roundNum}:{BattleResult(isPlayerWon)}");
        }

        private static string BattleResult(bool isPlayerWon)
        {
            return isPlayerWon ? VICTORY : DEFEAT;
        }

        public void DuelRerollPressed()
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent("RerollButtonPressed");
        }

        public void DuelUnitBought(string unitId)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:Unit:{unitId}:bought");
        }

        public void DuelUnitUpgraded(string unitId, int stars)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Duel:Unit:{unitId}:StarsUp{stars}");
        }

        public void ReportCampaignStageStart(int campaignNum, int stageNum)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Campaign:Chapter{campaignNum}:Stage{stageNum}:Start");
        }

        public void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Campaign:Chapter{campaignNum}:Stage{stageNum}:{BattleResult(isPlayerWon)}");
        }

        public void ScreenSwitched(string fromScreen, string toScreen)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Screen:{fromScreen}:Closed:{toScreen}");
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Screen:{toScreen}:Open:{fromScreen}");
        
        }
        public void BattlePassRewardGained(int level, BattlePassRewardType type)
        {
            var analyticsType = $"Get{type.AnalyticsId()}";
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"BattlePass:Reward:{level}:{analyticsType}");
        }
        public void ReportBattlePassLevelUp(int level, bool forHard)
        {
            var levelUpParam = forHard ? "LvlUpForHard" : "LvlUp";
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"BattlePass:LevelUp:{level}:{levelUpParam}");
        }

        public void BattlePassPremiumBought(int level)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"BattlePass:BuyPremiumBattlePass:{level}");
        }
        public void ReportGetFragmentsToUnlock(string unitId, int totalProgress)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Character:{unitId}:FragmentsToUnlock:{totalProgress}");
        }   
        public void ReportUnlockUnit(string unitId, int totalProgress)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Character:{unitId}:Unlock:{totalProgress}");
        }
        public void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Tutorial:{tutorialName}:{stepId}:{state.ToString()}");
        }
        public void ReportProductPurchase(string productId, int purchaseCount, int totalPurchaseNumber, int totalProgress)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Shop:{productId}:Count:{purchaseCount}:{totalProgress}");
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent($"Shop:{productId}:Number:{totalPurchaseNumber}:{totalProgress}");
        }
    }
}