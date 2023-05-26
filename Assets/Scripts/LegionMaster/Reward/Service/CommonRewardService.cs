using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Repository;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Model;
using UnityEngine;

namespace LegionMaster.Reward.Service
{
    public class CommonRewardService
    {
        private RewardSiteCollectionConfig _rewardSiteCollection;
        private RewardSiteStateRepository _repository;
        public CommonRewardService(RewardSiteCollectionConfig rewardSiteCollection, RewardSiteStateRepository repository)
        {
            _rewardSiteCollection = rewardSiteCollection;
            _repository = repository;
            InitRepository();
        }
        private void InitRepository()
        {
            var rewardSiteStateCollection = RewardSiteStateCollection;
            
            foreach (var rewardSiteConfig in _rewardSiteCollection.RewardSites) {
                var rewardSiteState = rewardSiteStateCollection.FindSiteState(rewardSiteConfig.SiteId);
                if (rewardSiteState == null) {
                    rewardSiteStateCollection.AddNewSite(rewardSiteConfig);
                } else {
                    rewardSiteState.AddMissingConfigs(rewardSiteConfig);
                }
            }
            _repository.Set(rewardSiteStateCollection);
        }
        public RewardItem CalculateReward(string rewardSiteId)
        {
            var stateCollection = RewardSiteStateCollection;
            
            var siteConfig = _rewardSiteCollection.GetRewardSiteConfig(rewardSiteId);
            var siteState = stateCollection.GetSiteState(rewardSiteId);
            siteState.IncrementAllCounter(siteConfig);
           
            var forcedRewards = siteState.GetForcedRewards(siteConfig);
            if (forcedRewards.Count != 0) {
                return CalculateForcedReward(forcedRewards, siteConfig, stateCollection);
            }
            var rewardConfigByWeight = siteConfig.GetRandomRewardByWeight(siteState);
            return CalculateReward(siteState.GetRewardState(rewardConfigByWeight.Id), rewardConfigByWeight, stateCollection);
        }
        private RewardItem CalculateForcedReward(List<RewardState> forcedRewards, RewardSiteConfig siteConfig, RewardSiteStateCollection stateCollection)
        {
            var sortedRewards = forcedRewards.OrderByDescending(it => it.RoundCounter).First();
            var rewardsWithEqualCounter = forcedRewards.Where(it => it.RoundCounter == sortedRewards.RoundCounter).ToList();
            var reward = rewardsWithEqualCounter.Random(); 
            return CalculateReward(reward, siteConfig.GetRewardConfig(reward.Id), stateCollection);
        }
        private RewardItem CalculateReward(RewardState rewardState, RewardConfig rewardConfig, RewardSiteStateCollection stateCollection)
        {
            if (rewardConfig.HasRounds) {
                rewardState.MarkAsTaken();
            }
            _repository.Set(stateCollection);
            var takenReward = new RewardItem(rewardConfig.Id, rewardConfig.Type, GetRewardCount(rewardConfig));
            return takenReward;
        }
        private int GetRewardCount(RewardConfig reward)
        {
            return Random.Range(reward.MinQuantity, reward.MaxQuantity + 1);
        }
        private RewardSiteStateCollection RewardSiteStateCollection => _repository.Get() ?? new RewardSiteStateCollection();
    }
}