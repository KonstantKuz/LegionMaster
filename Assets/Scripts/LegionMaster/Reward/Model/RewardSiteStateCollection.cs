using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Reward.Config;

namespace LegionMaster.Reward.Model
{
    public class RewardSiteStateCollection
    {
        private readonly List<RewardSiteState> _rewardSiteStates;
        
        public RewardSiteStateCollection()
        {
            _rewardSiteStates = new List<RewardSiteState>();
        }
        public RewardSiteState FindSiteState(string rewardSiteId)
        {
            return _rewardSiteStates.FirstOrDefault(it => it.RewardSiteId == rewardSiteId);
        } 
        public RewardSiteState GetSiteState(string rewardSiteId)
        {
            return FindSiteState(rewardSiteId) ?? throw new NullReferenceException($"RewardSiteState is null, rewardSiteId:= {rewardSiteId}");
        }

        public void Add(RewardSiteState rewardSiteState)
        {
            _rewardSiteStates.Add(rewardSiteState); 
        }

        public void AddNewSite(RewardSiteConfig rewardSiteConfig)
        {
            var rewardStates = rewardSiteConfig.Rewards.Select(it => new RewardState(it.Id)).ToList();
            Add(new RewardSiteState(rewardSiteConfig.SiteId, rewardStates));
        }
    }
}