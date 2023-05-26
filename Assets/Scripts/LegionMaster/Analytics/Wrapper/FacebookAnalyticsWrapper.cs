using System.Collections.Generic;
using Facebook.Unity;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;
using UnityEngine;

namespace LegionMaster.Analytics.Wrapper
{
    public class FacebookAnalyticsWrapper : IAnalyticsImpl
    {
        private const string BATTLE_NUMBER_PARAMETER = "battle_number";
        private const string DUEL_NUMBER_PARAMETER = "duel_number";
        private const string ROUND_NUMBER_PARAMETER = "round_number";
        private const string CAMPAIGN_NUMBER_PARAMETER = "campaign_number"; 
        private const string HYPER_CASUAL_NUMBER_PARAMETER = "hc_number";
        private const string STAGE_NUMBER_PARAMETER = "stage_number";
        private const string VICTORY_PARAMETER = "victory";
        private bool _isInitialized;

        public void Init()
        {
            Debug.Log("Starting initializing Facebook SDK");
            if (!FB.IsInitialized) {
                FB.Init(InitCallback, OnHideUnity);
            } else {
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized) {
                FB.Mobile.SetAdvertiserTrackingEnabled(true);
                FB.ActivateApp();
                _isInitialized = true;
                Debug.Log("Facebook SDK is Initialized");
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            Time.timeScale = !isGameShown ? 0 : 1;
        }

        public void ReportTest()
        {
            LogEvent("Test", null, new Dictionary<string, object>());
        }

        public void ReportBattleStart(int battleNum)
        {
            LogEvent("BattleStart", null, new Dictionary<string, object> {
                    {BATTLE_NUMBER_PARAMETER, battleNum}
            });
        }

        public void ReportBattleFirstStart(int totalProgress)
        {
            LogEvent("BattleStart", null, new Dictionary<string, object> {
                    {"totalProgress", totalProgress}
            });
        }

        public void ReportBattleEnd(int battleNum, bool isPlayerWon)
        {
            LogEvent("BattleEnd", null, new Dictionary<string, object>
            {
                {BATTLE_NUMBER_PARAMETER, battleNum},
                {VICTORY_PARAMETER, isPlayerWon}
            });
        }
        
        public void ReportDuelStart(int duelNum)
        {
            LogEvent("DuelStart", null, new Dictionary<string, object>
            {
                {DUEL_NUMBER_PARAMETER, duelNum}
            });
        }

        public void ReportDuelEnd(int duelNum, bool isPlayerWon)
        {
            LogEvent("DuelEnd", null, new Dictionary<string, object>
            {
                {DUEL_NUMBER_PARAMETER, duelNum},
                {VICTORY_PARAMETER, isPlayerWon}
            });
        }

        public void ReportCampaignStart(int campaignNum)
        {
            LogEvent("CampaignStart", null, new Dictionary<string, object>
            {
                {CAMPAIGN_NUMBER_PARAMETER, campaignNum}
            });
        }

        public void ReportCampaignEnd(int campaignNum, bool isPlayerWon)
        {
            LogEvent("CampaignEnd", null, new Dictionary<string, object>
            {
                {CAMPAIGN_NUMBER_PARAMETER, campaignNum},
                {VICTORY_PARAMETER, isPlayerWon}
            });
        }

        public void ReportHyperCasualBattleStart(int battleNum)
        {
            LogEvent("HCStart", null, new Dictionary<string, object> {
                    {HYPER_CASUAL_NUMBER_PARAMETER, battleNum}
            });
        }

        public void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon)
        {
            LogEvent("HCEnd", null, new Dictionary<string, object>
            {
                    {HYPER_CASUAL_NUMBER_PARAMETER, battleNum},
                    {VICTORY_PARAMETER, isPlayerWon}
            });
        }

        public void ReportStartNoUnits()
        {
            LogEvent("StartNoUnits");
        }  
        public void ReportUnitPlaceChanged()
        {
            LogEvent("UnitPlaceChanged");
        }

        public void ReportResourceGained(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
            LogEvent("ResourceGained", amount, BuildResourceEventParams(currencyType, acquisitionType, acquisitionPlace));
        }

        private static Dictionary<string, object> BuildResourceEventParams(CurrencyType currencyType, ResourceAcquisitionType acquisitionType, string acquisitionPlace)
        {
            return new Dictionary<string, object>
            {
                {"CurrencyType", currencyType.ToString()},
                {"AcquisitionType", acquisitionType.ToString()},
                {"AcquisitionPlace", acquisitionPlace}
            };
        }

        public void ReportResourceSpent(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
            LogEvent("ResourceSpent", amount, BuildResourceEventParams(currencyType, acquisitionType, acquisitionPlace));
        }

        public void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param)
        {
            var dict = new Dictionary<string, object>(param ?? new Dictionary<string, object>())
            {
                ["character"] = unitId
            };
            LogEvent(eventName, 0, dict);
        }

        public void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId)
        {
            LogEvent($"Quest_{eventId}", 0, new Dictionary<string, object>()
            {
                {"section_id", questSection.ToString()},
                {"quest_id", questId},
            });
        }

        public void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId)
        {
            LogEvent($"Quest_Section_{eventId}", 0, new Dictionary<string, object>()
            {
                {"section_id", rewardId.Type.ToString()},
                {"reward_id", rewardId.Points},
            });
        }

        public void ReportProfileEvent(PlayerCollectibles collectibles, int num, int delta)
        {
            
        }

        public void PlaceUnitByTap()
        {
            LogEvent("placeUnitByTap");
        }

        public void PlaceUnitByDragRight()
        {
            LogEvent("placeUnitByDragRight");
        }

        private void LogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
        {
            if (!_isInitialized)
            {
                //TODO: store events while fb sdk not initialized and send them after initialization
                Debug.LogWarning($"Facebook analytics event {logEvent} is lost, cause facebook sdk is not ready yet");
                return;
            }
            FB.LogAppEvent(logEvent, valueToSum, parameters);
        }

        public void ReportDuelRoundStart(int duelNum, int roundNum)
        {
            LogEvent("DuelRoundStart", null, new Dictionary<string, object>
            {
                {DUEL_NUMBER_PARAMETER, duelNum},
                {ROUND_NUMBER_PARAMETER, roundNum}
            });
        }

        public void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon)
        {
            LogEvent("DuelRoundEnd", null, new Dictionary<string, object>
            {
                {DUEL_NUMBER_PARAMETER, duelNum},
                {ROUND_NUMBER_PARAMETER, roundNum},
                {VICTORY_PARAMETER, isPlayerWon}
            });
        }
        
        public void DuelRerollPressed()
        {
            LogEvent("DuelRerollButtonPressed");
        }

        public void DuelUnitBought(string unitId)
        {
            LogEvent("DuelUnitBought", null,new Dictionary<string, object>
            {
                {"unit", unitId}
            });
        }

        public void DuelUnitUpgraded(string unitId, int stars)
        {
            LogEvent("DuelUnitUpgraded", null,new Dictionary<string, object>
            {
                {"unit", unitId},
                {"stars", stars}
            });
        }

        public void ReportCampaignStageStart(int campaignNum, int stageNum)
        {
            LogEvent("CampaignStageStart", null, new Dictionary<string, object>
            {
                {CAMPAIGN_NUMBER_PARAMETER, campaignNum},
                {STAGE_NUMBER_PARAMETER, stageNum}
            });
        }

        public void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon)
        {
            LogEvent("CampaignStageEnd", null, new Dictionary<string, object>
            {
                {CAMPAIGN_NUMBER_PARAMETER, campaignNum},
                {STAGE_NUMBER_PARAMETER, stageNum},
                {VICTORY_PARAMETER, isPlayerWon}
            });
        }
        
        public void ScreenSwitched(string fromScreen, string toScreen)
        {
            LogEvent("ScreenSwitched", null, new Dictionary<string, object> {
                    {"toScreen", toScreen},
                    {"fromScreen", fromScreen}
            });  
       
        }

        public void BattlePassRewardGained(int level, BattlePassRewardType type)
        {
            var analyticsType = $"Get{type.AnalyticsId()}";
            LogEvent("BattlePassRewardGained", null, new Dictionary<string, object> {
                    {"level", level},
                    {"type", analyticsType}
            });
        }

        public void ReportBattlePassLevelUp(int level, bool forHard)
        {
            var levelUpEvent = forHard ? "BattlePassLvlUpForHard" : "BattlePassLvlUp";
            LogEvent(levelUpEvent, null, new Dictionary<string, object> {
                    {"level", level},
            });
        }

        public void BattlePassPremiumBought(int level)
        {
            LogEvent("BuyPremiumBattlePass", null, new Dictionary<string, object> {
                    {"level", level},
            });
        }

        public void ReportGetFragmentsToUnlock(string unitId, int totalProgress)
        {
            LogEvent("FragmentsToUnlock", null, new Dictionary<string, object> {
                    {"unit", unitId},
                    {"Count", totalProgress}
            });
        }

        public void ReportUnlockUnit(string unitId, int totalProgress)
        {
            LogEvent("UnlockUnit", null, new Dictionary<string, object> {
                    {"unit", unitId},
                    {"Count", totalProgress}
            });
        }

        public void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state)
        {
            LogEvent("Tutorial", null, new Dictionary<string, object> {
                    {"name", tutorialName},
                    {"stepId", stepId},
                    {"state", state.ToString()}
            });
        }

        public void ReportProductPurchase(string productId, int purchaseCount, int totalPurchaseNumber, int totalProgress)
        {
            LogEvent("Shop", null, new Dictionary<string, object> {
                    {"productId", productId},
                    {"count", purchaseCount},
                    {"number", totalPurchaseNumber},
                    {"progress", totalProgress},
            });
        }
    }
}