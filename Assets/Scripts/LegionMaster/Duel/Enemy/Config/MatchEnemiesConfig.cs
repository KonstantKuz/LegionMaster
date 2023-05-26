using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Extension;

namespace LegionMaster.Duel.Enemy.Config
{
    public class MatchEnemiesConfig : ILoadableConfig
    {
        private const int RANDOM_MATCH_RANGE = 3;
        public IReadOnlyList<EnemyMatchConfig> Matches { get; private set; }
        
        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<EnemySpawnRoundConfig>(stream);
            Matches = parsed.Select(it => new EnemyMatchConfig(int.Parse(it.Key), it.Value))
                            .OrderBy(it => it.Match)
                            .ToList();
        }
        public EnemyMatchConfig GetMatchByIndex(int index)
        {
            return Matches[index];
        }

        public EnemyMatchConfig GetRandomMatch(int seed)
        {
            return Matches.Random(Math.Max(0, Matches.Count - RANDOM_MATCH_RANGE), Matches.Count, seed);
        }
        public int MatchCount => Matches.Count;
    }
}