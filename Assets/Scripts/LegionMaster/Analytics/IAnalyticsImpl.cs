using System.Collections.Generic;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;

namespace LegionMaster.Analytics
{
    public interface IAnalyticsImpl
    {
        void Init();
        void ReportTest();
        void ReportBattleStart(int battleNum);
        void ReportBattleFirstStart(int totalProgress);
        void ReportBattleEnd(int battleNum, bool isPlayerWon);
        void ReportDuelStart(int duelNum);
        void ReportDuelEnd(int duelNum, bool isPlayerWon);
        void ReportCampaignStart(int campaignNum);
        void ReportCampaignEnd(int campaignNum, bool isPlayerWon);
        void ReportHyperCasualBattleStart(int battleNum);
        void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon);
        
        void ReportStartNoUnits();     
        void ReportUnitPlaceChanged();
        void ReportResourceGained(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType, string acquisitionPlace);
        void ReportResourceSpent(CurrencyType currencyType, float amount, ResourceAcquisitionType acquisitionType, string acquisitionPlace);
        void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param);
        void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId);
        void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId);
        void ReportProfileEvent(PlayerCollectibles collectibles, int num, int delta);
        void PlaceUnitByTap();
        void PlaceUnitByDragRight();
        void ReportDuelRoundStart(int duelNum, int roundNum);
        void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon);
        void DuelRerollPressed();
        void DuelUnitBought(string unitId);
        void DuelUnitUpgraded(string unitId, int stars);
        void ReportCampaignStageStart(int campaignNum, int stageNum);
        void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon);
        void ScreenSwitched(string fromScreen, string toScreen);
        void BattlePassRewardGained(int level, BattlePassRewardType type);
        void ReportBattlePassLevelUp(int level, bool forHard);
        void BattlePassPremiumBought(int level);
        void ReportGetFragmentsToUnlock(string unitId, int totalProgress);
        void ReportUnlockUnit(string unitId, int totalProgress);
        void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state);
        void ReportProductPurchase(string productId, int purchaseCount, int totalPurchaseNumber, int totalProgress);
    }
}