using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Config.Pack
{
    [PublicAPI]
    public class PackConfigCollection : ILoadableConfig
    {
        private Dictionary<string, RewardPackConfig> _packs;

        public IEnumerable<RewardPackConfig> Packs => _packs.Values;
        public IEnumerable<RewardItem> GetRewards(string packId)
        {
            if (!_packs.ContainsKey(packId)) {
                throw new KeyNotFoundException($"RewardPackConfig is null, packId:= {packId}");
            }
            return _packs[packId].Rewards.Select(it => it.ToRewardItem());
        }

        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<RewardItemConfig>(stream);
            _packs = parsed.ToDictionary(it => it.Key, it => new RewardPackConfig {
                    PackId = it.Key,
                    Rewards = it.Value
            });
        }
    }
}