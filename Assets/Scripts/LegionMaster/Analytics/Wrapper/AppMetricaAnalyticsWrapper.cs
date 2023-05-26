using System.Collections.Generic;
using System.Globalization;
using GameAnalyticsSDK;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;
using UnityEngine;

namespace LegionMaster.Analytics.Wrapper
{
    enum BattleType
    {
        vsHuman,
        vsBot
    }

    public class AppMetricaAnalyticsWrapper : IAnalyticsImpl
    {
        private const string BATTLE_EVENT = "battle";
        private const string DUEL_EVENT = "duel";
        private const string DUEL_ROUND_EVENT = "duelRound";
        private const string CAMPAIGN_EVENT = "campaign";    
        private const string HYPER_CASUAL_EVENT = "hc";
        private const string CAMPAIGN_STAGE_EVENT = "campaignStage";
        private const string START_NO_UNIT_EVENT = "startNoUnits";
        private const string UNIT_PLACE_CHANGED_EVENT = "unitPlaceChanged";
        private const string RESOURCE_GAINED_EVENT = "resourceGained";
        private const string RESOURCE_SPENT_EVENT = "resourceSpent";

        private const string BATTLE_TYPE = "battleType";
        private const string BATTLE_NUM = "battleNum";
        private const string DUEL_NUM = "duelNum";
        private const string ROUND_NUM = "roundNum";
        private const string CAMPAIGN_NUM = "campaignNum";
        private const string STAGE_NUM = "stageNum";
        private const string BATTLE_STATUS = "battleStatus";
        private const string FLOW_TYPE = "flowType";
        private const string CURRENCY_TYPE = "currencyType";
        private const string AMOUNT = "amount";
        private const string ACQUISITION_TYPE = "acquisitionType";
        private const string ACQUISITION_PLACE = "acquisitionPlace";
        private const string CHARACTER = "character";
        private const string PLACE_UNIT_BY_TAP = "placeUnitByTap";
        private const string PLACE_UNIT_BY_DRAG_RIGHT = "placeUnitByDragRight";

        private const string BATTLE_STATUS_START = "start";
        private const string BATTLE_STATUS_COMPLETE = "complete";
        private const string BATTLE_STATUS_FAIL = "fail";

        private bool _isInitialized;

        public void Init()
        {
            Debug.Log("Starting initializing AppMetrica");
            AppMetrica.Setup(OnActivation);
        }
        
        private void OnActivation(YandexAppMetricaConfig config)
        {
            _isInitialized = true;
            Debug.Log("AppMetrica is Initialized");
        }
        
        public void ReportTest()
        {
            ReportEvent("Test", new Dictionary<string, object>());
        }
        
        public void ReportBattleStart(int battleNum)
        {
            ReportEvent(BATTLE_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, battleNum.ToString()},
                {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportBattleFirstStart(int totalProgress)
        {
            ReportEvent(BATTLE_EVENT, new Dictionary<string, object> {
                    {"totalProgress", totalProgress.ToString()}
            });
        }

        private void ReportEvent(string message, Dictionary<string, object> parameters)
        {
            if (!_isInitialized)
            {
                //TODO: store events while appmetrica sdk not initialized and send them after initialization
                ShowLostEventWarning(message);
                return;
            }

            AppMetrica.Instance.ReportEvent(message, parameters);
        }

        private static void ShowLostEventWarning(string message)
        {
            Debug.LogWarning($"AppMetrica analytics event {message} is lost, cause appmetrica sdk is not ready yet");
        }

        private void ReportEvent(string message) {
            if (!_isInitialized)
            {
                //TODO: store events while appmetrica sdk not initialized and send them after initialization
                ShowLostEventWarning(message);
                return;
            }

            AppMetrica.Instance.ReportEvent(message);
        }

        public void ReportBattleEnd(int battleNum, bool isPlayerWon)
        {
            var battleStatus = isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL;

            ReportEvent(BATTLE_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, battleNum.ToString()},
                {BATTLE_STATUS, battleStatus}
            });
        }

        public void ReportDuelStart(int duelNum)
        {
            ReportEvent(DUEL_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, duelNum.ToString()},
                {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportDuelEnd(int duelNum, bool isPlayerWon)
        {
            ReportEvent(DUEL_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, duelNum.ToString()},
                {BATTLE_STATUS, isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL}
            });
        }

        public void ReportCampaignStart(int campaignNum)
        {
            ReportEvent(CAMPAIGN_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, campaignNum.ToString()},
                {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportCampaignEnd(int campaignNum, bool isPlayerWon)
        {
            ReportEvent(CAMPAIGN_EVENT, new Dictionary<string, object>
            {
                {BATTLE_TYPE, BattleType.vsBot.ToString()},
                {BATTLE_NUM, campaignNum.ToString()},
                {BATTLE_STATUS, isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL}
            });
        }

        public void ReportHyperCasualBattleStart(int battleNum)
        {
            ReportEvent(HYPER_CASUAL_EVENT, new Dictionary<string, object>
            {
                    {BATTLE_TYPE, BattleType.vsBot.ToString()},
                    {BATTLE_NUM, battleNum.ToString()},
                    {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon)
        {
            var battleStatus = isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL;

            ReportEvent(HYPER_CASUAL_EVENT, new Dictionary<string, object>
            {
                    {BATTLE_TYPE, BattleType.vsBot.ToString()},
                    {BATTLE_NUM, battleNum.ToString()},
                    {BATTLE_STATUS, battleStatus}
            });
        }

        public void ReportStartNoUnits()
        {
            ReportEvent(START_NO_UNIT_EVENT);
        }

        public void ReportUnitPlaceChanged()
        {
            ReportEvent(UNIT_PLACE_CHANGED_EVENT);
        }

        public void PlaceUnitByTap()
        {
            ReportEvent(PLACE_UNIT_BY_TAP);
        }

        public void PlaceUnitByDragRight()
        {
            ReportEvent(PLACE_UNIT_BY_DRAG_RIGHT);
        }
        
        public void ReportResourceGained(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
            ReportEvent(RESOURCE_GAINED_EVENT,
                BuildResourceEventParams(GAResourceFlowType.Source.ToString(), currencyType, amount, acquisitionType,
                    acquisitionPlace));
        }
        
        public void ReportResourceSpent(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType,
            string acquisitionPlace)
        {
            ReportEvent(RESOURCE_SPENT_EVENT,
                BuildResourceEventParams(GAResourceFlowType.Sink.ToString(), currencyType, amount, acquisitionType,
                    acquisitionPlace));
        }

        public void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param)
        {
            ReportEvent(eventName, new Dictionary<string, object>(param ?? new Dictionary<string, object>())
            {
                [CHARACTER] = unitId
            });
        }

        public void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId)
        {
            ReportEvent($"quest:sectionid:{questSection}:{questId}:{eventId}");
        }

        public void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId)
        {
            ReportEvent($"quest:sectionid:{rewardId.Type}:{rewardId.Points}:{eventId}");
        }
        
        public void ReportProfileEvent(PlayerCollectibles collectible, int num, int delta)
        {
            if (!_isInitialized)
            {
                ShowLostEventWarning("player profile report");
                return;
            }

            var eventKey = AnalyticsCollectibles.ToAnalytics(collectible);
            if (eventKey == null)
            {
                return;
            }
            var attributes = new List<YandexAppMetricaUserProfileUpdate>
            {
                new YandexAppMetricaNumberAttribute(eventKey).WithValue(num)
            };
            
            var userProfile = new YandexAppMetricaUserProfile();
            userProfile.ApplyFromArray(attributes);
            AppMetrica.Instance.SetUserProfileID("id");
            AppMetrica.Instance.ReportUserProfile(userProfile);

            if (collectible != PlayerCollectibles.Battle) {
                AppMetrica.Instance.ReportEvent($"{eventKey}:{delta}");
            }
        }

        private static Dictionary<string, object> BuildResourceEventParams(string flowType, CurrencyType currencyType,
            float amount, ResourceAcquisitionType acquisitionType, string acquisitionPlace)
        {
            return new Dictionary<string, object>
            {
                {FLOW_TYPE, flowType},
                {CURRENCY_TYPE, currencyType.ToString()},
                {AMOUNT, amount.ToString(CultureInfo.InvariantCulture)},
                {ACQUISITION_TYPE, acquisitionType.ToString()},
                {ACQUISITION_PLACE, acquisitionPlace}
            };
        }

        public void ReportDuelRoundStart(int duelNum, int roundNum)
        {
            ReportEvent(DUEL_ROUND_EVENT, new Dictionary<string, object>
            {
                {DUEL_NUM, duelNum.ToString()},
                {ROUND_NUM, roundNum.ToString()},
                {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon)
        {
            ReportEvent(DUEL_ROUND_EVENT, new Dictionary<string, object>
            {
                {DUEL_NUM, duelNum.ToString()},
                {ROUND_NUM, roundNum.ToString()},
                {BATTLE_STATUS, isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL}
            });
        }

        public void DuelRerollPressed()
        {
            ReportEvent("DuelRerollButtonPressed");
        }

        public void DuelUnitBought(string unitId)
        {
            ReportEvent("DuelUnitBought", new Dictionary<string, object>
            {
                {"unit", unitId}
            });
        }

        public void DuelUnitUpgraded(string unitId, int stars)
        {
            ReportEvent("DuelUnitUpgraded", new Dictionary<string, object>
            {
                {"unit", unitId},
                {"stars", stars}
            });
        }

        public void ReportCampaignStageStart(int campaignNum, int stageNum)
        {
            ReportEvent(CAMPAIGN_STAGE_EVENT, new Dictionary<string, object>
            {
                {CAMPAIGN_NUM, campaignNum.ToString()},
                {STAGE_NUM, stageNum.ToString()},
                {BATTLE_STATUS, BATTLE_STATUS_START}
            });
        }

        public void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon)
        {
            ReportEvent(CAMPAIGN_STAGE_EVENT, new Dictionary<string, object>
            {
                {CAMPAIGN_NUM, campaignNum.ToString()},
                {STAGE_NUM, stageNum.ToString()},
                {BATTLE_STATUS, isPlayerWon ? BATTLE_STATUS_COMPLETE : BATTLE_STATUS_FAIL}
            });
        }
        
        public void ScreenSwitched(string fromScreen, string toScreen)
        {
            ReportEvent("ScreenSwitched", new Dictionary<string, object> {
                    {"toScreen", toScreen},
                    {"fromScreen", fromScreen}
            });  
        
        }

        public void BattlePassRewardGained(int level, BattlePassRewardType type)
        {
            var analyticsType = $"Get{type.AnalyticsId()}";
            ReportEvent("BattlePassRewardGained", new Dictionary<string, object> {
                    {"level", level},
                    {"type", analyticsType}
            });
        }

        public void ReportBattlePassLevelUp(int level, bool forHard)
        {
            var levelUpEvent = forHard ? "BattlePassLvlUpForHard" : "BattlePassLvlUp";
            ReportEvent(levelUpEvent, new Dictionary<string, object> {
                    {"level", level},
            });
        }

        public void BattlePassPremiumBought(int level)
        {
            ReportEvent("BuyPremiumBattlePass", new Dictionary<string, object> {
                    {"level", level},
            });
        }

        public void ReportGetFragmentsToUnlock(string unitId, int totalProgress)
        {
            ReportEvent("FragmentsToUnlock", new Dictionary<string, object> {
                    {"unit", unitId},
                    {"Count", totalProgress}
            });
        }

        public void ReportUnlockUnit(string unitId, int totalProgress)
        {
            ReportEvent("UnlockUnit", new Dictionary<string, object> {
                    {"unit", unitId},
                    {"Count", totalProgress}
            });
        }

        public void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state)
        {
            ReportEvent("Tutorial", new Dictionary<string, object> {
                    {"name", tutorialName},
                    {"stepId", stepId},
                    {"state", state.ToString()}
            });
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
    }
}