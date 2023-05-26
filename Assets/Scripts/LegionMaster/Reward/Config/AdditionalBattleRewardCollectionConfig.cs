using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Reward.Config
{
    [PublicAPI]
    public class AdditionalBattleRewardCollectionConfig : ILoadableConfig
    {
        private Dictionary<string, IReadOnlyList<AdditionalBattleRewardConfig>> _rewards;

        public void Load(Stream stream)
        {
            _rewards = new CsvSerializer().ReadNestedTable<AdditionalBattleRewardConfig>(stream);
        }

        public IEnumerable<AdditionalBattleRewardConfig> GetRewards(int battleId)
        {
            var key = battleId.ToString();
            return _rewards.ContainsKey(key) ? _rewards[key] : Enumerable.Empty<AdditionalBattleRewardConfig>();
        }
    }
}