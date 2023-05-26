using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Reward.Config;
using SuperMaxim.Core.Extensions;

namespace LegionMaster.Reward.Model
{
    public class RewardSiteState
    {
        public readonly string RewardSiteId;
        private readonly List<RewardState> _rewardStates;

        public RewardSiteState(string rewardSiteId, List<RewardState> rewardStates)
        {
            RewardSiteId = rewardSiteId;
            _rewardStates = rewardStates;
        }

        public RewardState FindRewardState(string id)
        {
            return _rewardStates.FirstOrDefault(it => it.Id == id);
        }

        public bool ContainsRewardState(string id)
        {
            return FindRewardState(id) != null;
        }

        public RewardState GetRewardState(string id)
        {
            return FindRewardState(id) ?? throw new NullReferenceException($"RewardState is null, rewardId:= {id}");
        }

        public void Add(RewardState rewardState)
        {
            _rewardStates.Add(rewardState);
        }

        public void IncrementAllCounter(RewardSiteConfig rewardSiteConfig)
        {
            rewardSiteConfig.GetWithRoundsRewards()
                           .ForEach(it => {
                               var rewardState = GetRewardState(it.Id);
                               rewardState.IncrementCounter(it);
                           });
        }

        public List<RewardState> GetForcedRewards(RewardSiteConfig rewardSiteConfig)
        {
            return rewardSiteConfig.GetWithRoundsRewards()
                                  .Where(it => {
                                      var rewardState = GetRewardState(it.Id);
                                      return !rewardState.TakenThisRound && rewardState.RoundCounter >= it.Rounds;
                                  })
                                  .Select(it => GetRewardState(it.Id))
                                  .ToList();
        }

        public void AddMissingConfigs(RewardSiteConfig rewardSiteConfig)
        {
            rewardSiteConfig.Rewards.ForEach(it => {
                if (!ContainsRewardState(it.Id)) {
                    Add(new RewardState(it.Id));
                }
            });
        }
    }
}