using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;
using UnityEngine;

namespace LegionMaster.Analytics.Wrapper
{
    //TODO: implement all methods
    public class GoogleAnalyticsWrapper : IAnalyticsImpl
    {
        private FirebaseApp _app;
        public void Init()
        {
            Debug.Log("Starting initializing Google Analytics");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    _app = FirebaseApp.DefaultInstance;
                    Debug.Log("Google Analytics is initialized");
                } else {
                    Debug.LogError($"Failed to initialize Google Analytics: Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        public void ReportTest()
        {
            
        }

        public void ReportBattleStart(int battleNum)
        {
        }

        public void ReportBattleFirstStart(int totalProgress)
        {
        }

        public void ReportBattleEnd(int battleNum, bool isPlayerWon)
        {
        }

        public void ReportDuelStart(int duelNum)
        {
        }

        public void ReportDuelEnd(int duelNum, bool isPlayerWon)
        {
        }

        public void ReportCampaignStart(int campaignNum)
        {
        }

        public void ReportCampaignEnd(int campaignNum, bool isPlayerWon)
        {
        }

        public void ReportHyperCasualBattleStart(int battleNum)
        {
            
        }

        public void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon)
        {
            
        }

        public void ReportStartNoUnits()
        {
        }

        public void ReportUnitPlaceChanged()
        {
        }

        public void ReportResourceGained(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
        }

        public void ReportResourceSpent(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
        }

        public void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param)
        {
        }

        public void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId)
        {
        }

        public void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId)
        {
        }

        public void ReportProfileEvent(PlayerCollectibles collectibles, int num, int delta)
        {
        }

        public void PlaceUnitByTap()
        {
        }

        public void PlaceUnitByDragRight()
        {
        }

        public void ReportDuelRoundStart(int duelNum, int roundNum)
        {
        }

        public void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon)
        {
        }

        public void DuelRerollPressed()
        {
        }

        public void DuelUnitBought(string unitId)
        {
        }

        public void DuelUnitUpgraded(string unitId, int stars)
        {
        }

        public void ReportCampaignStageStart(int campaignNum, int stageNum)
        {
        }

        public void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon)
        {
        }

        public void ScreenSwitched(string fromScreen, string toScreen)
        {
        }

        public void BattlePassRewardGained(int level, BattlePassRewardType type)
        {
        }

        public void ReportBattlePassLevelUp(int level, bool forHard)
        {
        }

        public void BattlePassPremiumBought(int level)
        {
        }

        public void ReportGetFragmentsToUnlock(string unitId, int totalProgress)
        {
        }

        public void ReportUnlockUnit(string unitId, int totalProgress)
        {
        }

        public void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state)
        {
        }

        public void ReportProductPurchase(string productId, int purchaseCount, int totalPurchaseNumber, int totalProgress)
        {
            ReportEvent("Shop", new Dictionary<string, object> {
                {"productId", productId},
                {"count", purchaseCount},
                {"number", totalPurchaseNumber},
                {"progress", totalProgress},
            });
        }

        private void ReportEvent(string eventName, Dictionary<string,object> eventParams)
        {
            if (_app == null)
            {
                Debug.LogWarning("GoogleAnalytics is not initialized yet - skipping event");
                return;
            }
            
            FirebaseAnalytics.LogEvent(
                eventName, 
                eventParams.Select(it => CreateParameter(it.Key, it.Value)).ToArray());
        }

        private static Parameter CreateParameter(string id, object value)
        {
            return value switch
            {
                int intValue => new Parameter(id, intValue),
                float floatValue => new Parameter(id, floatValue),
                double doubleValue => new Parameter(id, doubleValue),
                string stringValue => new Parameter(id, stringValue),
                _ => new Parameter(id, value.ToString())
            };
        }
    }
}