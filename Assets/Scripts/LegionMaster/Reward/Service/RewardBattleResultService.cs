using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Location.Session.Model;
using LegionMaster.Player.Inventory.Config;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Model;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace LegionMaster.Reward.Service
{
    [PublicAPI]
    public class RewardBattleResultService
    {
        private const int DEFAULT_RATING_RANK_MULTIPLIER = 1; // TODO: replace after adding rating
        
        [Inject(Id = Configs.REWARD_BATTLE_RESULT)]
        private RewardBattleCollectionConfig _battleRewards;
        [Inject(Id = Configs.REWARD_DUEL_RESULT)]
        private RewardBattleCollectionConfig _duelRewards;     
        [Inject(Id = Configs.REWARD_CAMPAIGN_RESULT)]
        private RewardBattleCollectionConfig _campaignRewards;
        
        [Inject]
        private StringKeyedConfigCollection<ResourceConfig> _resourceCollectionStringKeyedConfig;
        [Inject]
        private CommonRewardService _commonRewardService;
        [Inject] 
        private AdditionalBattleRewardCollectionConfig _additionalBattleReward;
        
        public List<RewardItem> CalculateRewards(BattleResult battleResult, BattleSession battleSession)
        {
            var takenRewards = GetCurrencyRewards(battleResult, battleSession);
            takenRewards.AddRange(GetLootReward(battleResult, battleSession));
            return takenRewards;
        }

        private IEnumerable<RewardItem> GetLootReward(BattleResult battleResult, BattleSession battleSession)
        {
            if (battleResult != BattleResult.WIN) {
                return new RewardItem[] { };
            }
            return battleSession.Mode switch {
                    GameMode.HyperCasual => new RewardItem[] { },
                    GameMode.Battle => GetBattleWinReward(battleSession),
                    GameMode.Duel => GetDuelWinReward(),
                    GameMode.Campaign => GetCampaignWinReward(),
                    _ => throw new ArgumentOutOfRangeException(nameof(battleSession.Mode), battleSession.Mode, null)
            };
        }

        private IEnumerable<RewardItem> GetBattleWinReward(BattleSession battleSession)
        {
            return _additionalBattleReward.GetRewards(battleSession.BattleId)
                                          .Select(config => new RewardItem(config.RewardId, config.Type, config.Count));
        }

        private RewardItem[] GetDuelWinReward()
        {
            return new []{_commonRewardService.CalculateReward(RewardSiteIds.DUEL_REWARDS)};
        }   
        private RewardItem[] GetCampaignWinReward()
        {
            return new []{_commonRewardService.CalculateReward(RewardSiteIds.CAMPAIGN_REWARDS)};
        }

        private List<RewardItem> GetCurrencyRewards(BattleResult battleResult, BattleSession battleSession)
        {
            var takenRewards = new List<RewardItem>();
            var resultConfig = GetRewardConfig(battleSession.Mode).GetRewardBattleResultConfig(battleResult);
            foreach (var reward in resultConfig.Rewards)
            {
                int currencyValue = _resourceCollectionStringKeyedConfig.Get(reward.Id).Value;
                int countReward = (int)((GetRewardCount(reward) + battleSession.EnemiesKilled * reward.CoEnemiesKilled) *
                    DEFAULT_RATING_RANK_MULTIPLIER
                    / currencyValue * reward.FactorBattleResult);
                var takenReward = new RewardItem(reward.Id, reward.Type, Mathf.Max(0, countReward));
                takenRewards.Add(takenReward);
            }

            return takenRewards;
        }

        private RewardBattleCollectionConfig GetRewardConfig(GameMode gameMode) => gameMode switch
        {
                GameMode.HyperCasual => _battleRewards,
                GameMode.Battle => _battleRewards,
                GameMode.Duel => _duelRewards, 
                GameMode.Campaign => _campaignRewards, 
                _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };

        private int GetRewardCount(RewardBattleConfig reward)
        {
            return Random.Range(reward.MinNumber, reward.MaxNumber + 1);
        }
    }
}