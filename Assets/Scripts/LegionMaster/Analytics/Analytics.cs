using System;
using System.Collections.Generic;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Quest.Model;
using LegionMaster.UI.Screen;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace LegionMaster.Analytics
{
    public class Analytics
    {
        private readonly ICollection<IAnalyticsImpl> _impls;
        
        private string _acquisitionPlace;   
        private ResourceAcquisitionType? _acquisitionType;
        private ScreenId _activeAnalyticsScreen;

        public Analytics(ICollection<IAnalyticsImpl> impls)
        {
            _impls = impls;
        }

        public void Init()
        {
            Debug.Log("Initializing Analytics");
            foreach (var impl in _impls)
            {
                impl.Init();
            }
        }
        public void ReportTest()
        {
            _impls.ForEach(it => it.ReportTest());
        }
        public void ReportBattleStart(int battleNum, int battleCount, int totalProgress)
        {
            _impls.ForEach(it => it.ReportBattleStart(IndexWithOne(battleNum)));
            if (battleCount < 1) {
                ReportBattleFirstStart(totalProgress);
            }
        } 
        public void ReportBattleFirstStart(int totalProgress)
        {
            _impls.ForEach(it => it.ReportBattleFirstStart(totalProgress));
        }

        public void ReportBattleEnd(int battleNum, bool isPlayerWon)
        {
            _impls.ForEach(it => it.ReportBattleEnd(IndexWithOne(battleNum), isPlayerWon));
        }

        public void ReportDuelStart(int duelNum)
        {
            _impls.ForEach(it => it.ReportDuelStart(IndexWithOne(duelNum)));
        }

        public void ReportDuelEnd(int duelNum, bool isPlayerWon)
        {
            _impls.ForEach(it => it.ReportDuelEnd(IndexWithOne(duelNum), isPlayerWon));
        }

        public void ReportCampaignStart(int campaignNum)
        { 
            _impls.ForEach(it => it.ReportCampaignStart(IndexWithOne(campaignNum)));
        }

        public void ReportCampaignEnd(int campaignNum, bool isPlayerWon)
        { 
            _impls.ForEach(it => it.ReportCampaignEnd(IndexWithOne(campaignNum), isPlayerWon));
        }
        public void ReportHyperCasualBattleStart(int battleNum)
        {
            _impls.ForEach(it => it.ReportHyperCasualBattleStart(IndexWithOne(battleNum)));
        }
        public void ReportHyperCasualBattleEnd(int battleNum, bool isPlayerWon)
        {
            _impls.ForEach(it => it.ReportHyperCasualBattleEnd(IndexWithOne(battleNum), isPlayerWon));
        }

        public void ReportStartNoUnits()
        {
            _impls.ForEach(it => it.ReportStartNoUnits());
        }   
        public void ReportUnitPlaceChanged()
        {
            _impls.ForEach(it => it.ReportUnitPlaceChanged());
        }

        public void ReportResourceGained(CurrencyType currencyType, float amount)
        {
            _impls.ForEach(it => it.ReportResourceGained(currencyType, amount, AcquisitionType, AcquisitionPlace));
        }

        public void ReportResourceSpent(CurrencyType currencyType, float amount)
        {
            _impls.ForEach(it => it.ReportResourceSpent(currencyType, amount, AcquisitionType, AcquisitionPlace));
        }

        public void ReportUnitEvent(string unitId, string eventName, Dictionary<string, object> param)
        {
            _impls.ForEach(it => it.ReportUnitEvent(unitId, eventName, param));
        }

        /*
         *  use it like:
         * using (_analytics.SetAcquisitionProperties("screenA", ResourceAcquisitionType.Boost))
         * {
         *      call here any logic that dealing with resource
         * }
         */
        public IDisposable SetAcquisitionProperties(string acquisitionPlace, ResourceAcquisitionType acquisitionType)
        {
            Assert.IsNull(_acquisitionPlace, "shouldn't be called inside other SetAcquisitionProperties");
            _acquisitionPlace = acquisitionPlace;     
            _acquisitionType = acquisitionType;
            return Disposable.Create(() => {
                Assert.IsNotNull(_acquisitionPlace, "probably called inside other SetAcquisitionProperties");
                _acquisitionPlace = null;
                _acquisitionType = null;
            });
        }

        private string AcquisitionPlace
        {
            get
            {
                if (_acquisitionPlace == null)
                {
                    Debug.LogError("AcquisitionPlace is not set");
                }
                return _acquisitionPlace ?? "NotSpecified";
            }
        } 
        private ResourceAcquisitionType AcquisitionType
        {
            get
            {
                if (_acquisitionType == null)
                {
                    Debug.LogError("AcquisitionType is not set");
                }
                return _acquisitionType ?? ResourceAcquisitionType.None;
            }
        }

        public void ReportQuestEvent(QuestSectionType questSection, string questId, string eventId)
        {
            _impls.ForEach(it => it.ReportQuestEvent(questSection, questId, eventId));
        }

        public void ReportQuestSectionEvent(QuestSectionRewardId rewardId, string eventId)
        {
            _impls.ForEach(it => it.ReportQuestSectionEvent(rewardId, eventId));
        }

        public void ReportProfileEvent(PlayerCollectibles collectible, int num, int delta)
        {
            _impls.ForEach(it => it.ReportProfileEvent(collectible, num, delta));
        }

        public void PlaceUnitByTap()
        {
            _impls.ForEach(it => it.PlaceUnitByTap());
        }

        public void PlaceUnitByDragRight()
        {
            _impls.ForEach(it => it.PlaceUnitByDragRight());
        }

        public void ReportDuelRoundStart(int duelNum, int roundNum)
        {
            _impls.ForEach(it => it.ReportDuelRoundStart(IndexWithOne(duelNum), roundNum));
        }
        
        public void ReportDuelRoundEnd(int duelNum, int roundNum, bool isPlayerWon)
        {
            _impls.ForEach(it => it.ReportDuelRoundEnd(IndexWithOne(duelNum), roundNum, isPlayerWon));
        }

        public void DuelRerollPressed()
        {
            _impls.ForEach(it => it.DuelRerollPressed());
        }

        public void DuelUnitBought(string unitId)
        {
            _impls.ForEach(it => it.DuelUnitBought(unitId));
        }

        public void DuelUnitUpgraded(string unitId, int stars)
        {
            _impls.ForEach(it => it.DuelUnitUpgraded(unitId, stars));
        }
        
        public void ReportCampaignStageStart(int campaignNum, int stageNum)
        {
            _impls.ForEach(it => it.ReportCampaignStageStart(IndexWithOne(campaignNum), stageNum));
        }

        public void ReportCampaignStageEnd(int campaignNum, int stageNum, bool isPlayerWon)
        {
            _impls.ForEach(it => it.ReportCampaignStageEnd(IndexWithOne(campaignNum), stageNum, isPlayerWon));
        }

        public void ScreenSwitched(ScreenId toScreen)
        {
            if (!toScreen.CanBeInAnalytics()) {
                return;
            }
            if (_activeAnalyticsScreen == ScreenId.None) {
                _activeAnalyticsScreen = toScreen;
                return;
            }
            _impls.ForEach(it => it.ScreenSwitched(_activeAnalyticsScreen.AnalyticsId(), toScreen.AnalyticsId()));
            _activeAnalyticsScreen = toScreen;
        }
        public void BattlePassRewardGained(BattlePassRewardId rewardId)
        {
            _impls.ForEach(it => it.BattlePassRewardGained(rewardId.Level, rewardId.Type));
        }   
        public void ReportBattlePassLevelUp(int level, bool bought)
        { 
            _impls.ForEach(it => it.ReportBattlePassLevelUp(level, false));
            if (bought) {
                _impls.ForEach(it => it.ReportBattlePassLevelUp(level, true));
            }
        }     
        public void BattlePassPremiumBought(int level)
        { 
            _impls.ForEach(it => it.BattlePassPremiumBought(level));
        }
        public void ReportGetFragmentsToUnlock(string unitId, PlayerProgress playerProgress)
        {
            _impls.ForEach(it => it.ReportGetFragmentsToUnlock(unitId, playerProgress.TotalProgress));
        }   
        public void ReportUnlockUnit(string unitId, PlayerProgress playerProgress)
        {
            _impls.ForEach(it => it.ReportUnlockUnit(unitId, playerProgress.TotalProgress));
        }
        public void ReportTutorialStep(string tutorialName, int stepId, TutorialStepState state)
        {
            _impls.ForEach(it => it.ReportTutorialStep(tutorialName, stepId, state));
        }    
        public void ReportProductPurchase(string productId, int purchaseCount, int totalPurchaseNumber, int totalProgress)
        {
            _impls.ForEach(it => it.ReportProductPurchase(productId, purchaseCount, totalPurchaseNumber, totalProgress));
        } 
        
        //fix indexing that start with 0 to start with 1
        private static int IndexWithOne(int value) => value + 1;
      
    }
}