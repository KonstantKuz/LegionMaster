using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Location.Session.Model;

namespace LegionMaster.Reward.Config
{
    [PublicAPI]
    public class RewardBattleCollectionConfig: ILoadableConfig
    {
        private Dictionary<BattleResult, RewardBattleResultConfig> _rewards;
        
        public RewardBattleResultConfig GetRewardBattleResultConfig(BattleResult battleResult)
        {
            return _rewards[battleResult];
        }

        public void Load(Stream stream)
        {
            _rewards = new Dictionary<BattleResult, RewardBattleResultConfig>();
            var parsed = new CsvSerializer().ReadNestedTable<RewardBattleConfig>(stream);
            foreach (BattleResult result in Enum.GetValues(typeof(BattleResult)))
            {
                if (!parsed.ContainsKey(result.ToString()))
                {
                    throw new Exception($"Missing battle reward config for result {result}");
                }

                _rewards[result] = new RewardBattleResultConfig
                {
                    BattleResult = result,
                    Rewards = parsed[result.ToString()]
                };
            }
        }
    }
}