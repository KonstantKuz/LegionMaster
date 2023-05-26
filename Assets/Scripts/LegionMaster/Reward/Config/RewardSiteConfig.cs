using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LegionMaster.Extension;
using LegionMaster.Reward.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class RewardSiteConfig
    {
        [DataMember(Name = "SiteId")]
        public string SiteId;
        [DataMember(Name = "Rewards")]
        public IReadOnlyList<RewardConfig> Rewards;

        public RewardConfig GetRewardConfig(string id)
        {
            return Rewards.FirstOrDefault(it => it.Id == id) ?? throw new NullReferenceException($"RewardConfig is null, rewardId:= {id}");
        }

        public List<RewardConfig> GetWithRoundsRewards()
        {
            return Rewards.Where(it => it.HasRounds).ToList();
        }

        public RewardConfig GetRandomRewardByWeight(RewardSiteState rewardSiteState)
        {
            var rewardConfigs = Rewards.Where(it => !rewardSiteState.GetRewardState(it.Id).TakenThisRound).ToList();
            
            int sumWeight = rewardConfigs.Sum(it => it.Weight);
            int probability = Random.Range(1, sumWeight + 1);
            foreach (var reward in rewardConfigs) {
                if (reward.Weight >= probability) {
                    return reward;
                }
                probability -= reward.Weight;
            }
            Debug.LogWarning("generation error by weight");
            return rewardConfigs.Random();
        }
    }
}