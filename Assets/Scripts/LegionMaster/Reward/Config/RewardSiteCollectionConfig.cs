using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Reward.Config
{
    [PublicAPI]
    public class RewardSiteCollectionConfig: ILoadableConfig
    {
        private Dictionary<string, RewardSiteConfig> _rewardSites;

        public IEnumerable<RewardSiteConfig> RewardSites => _rewardSites.Values;

        public RewardSiteConfig GetRewardSiteConfig(string rewardSiteId)
        {
            if (!_rewardSites.ContainsKey(rewardSiteId)) {
                throw new KeyNotFoundException($"RewardSiteId not found in RewardSiteConfig, rewardSiteId:= {rewardSiteId}");
            }
            return _rewardSites[rewardSiteId];
        }

        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<RewardConfig>(stream);
            _rewardSites = parsed.ToDictionary(it => it.Key, it => new RewardSiteConfig
            {
                SiteId = it.Key,
                Rewards = it.Value
            });
        }
    }
}