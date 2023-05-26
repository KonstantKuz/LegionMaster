using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.BattlePass.Model;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Reward.Model;

namespace LegionMaster.BattlePass.Config
{
    [PublicAPI]
    public class BattlePassConfigList : ILoadableConfig
    {
        public IReadOnlyList<BattlePassConfig> Items { get; private set; }

        public BattlePassConfigList()
        {
           
        }
        public BattlePassConfigList(IReadOnlyList<BattlePassConfig> items)
        {
            Items = items;
        }  
        public void Load(Stream stream)
        {
            Items = new CsvSerializer().ReadObjectArray<BattlePassConfig>(stream);
        }
 
        public BattlePassConfig GetConfigByLevel(int level)
        {
            return Items.FirstOrDefault(it => it.Level == level) ?? throw new NullReferenceException($"BattlePassConfigList is null by level:= {level}");
        }

        [CanBeNull]
        public RewardItem FindRewardById(BattlePassRewardId rewardId)
        {
            return GetConfigByLevel(rewardId.Level).GetReward(rewardId.Type);
        }

        public BattlePassConfig GetMaxLevelConfig()
        {
            return Items.OrderByDescending(it => it.Level).First();
        }
        public BattlePassConfig GetMinLevelConfig()
        {
            return Items.OrderBy(it => it.Level).First();
        }
    }
}