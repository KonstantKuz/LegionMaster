using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Extension;

namespace LegionMaster.Enemy.Config
{
    [PublicAPI]
    public class EnemiesConfig : ILoadableConfig
    {
        private const int RANDOM_SQUAD_RANGE = 3;

        public IReadOnlyList<EnemySquadConfig> EnemySquadConfigs { get; private set; }

        public EnemySquadConfig GetEnemySquad(int id)
        {
            return EnemySquadConfigs.FirstOrDefault(it => it.Id == id) ?? throw new NullReferenceException($"EnemySquadConfig is null, id:= {id}");
        }

        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<EnemySpawnConfig>(stream);
            EnemySquadConfigs = parsed.Select(it => new EnemySquadConfig(int.Parse(it.Key), it.Value))
                                      .OrderBy(it => it.Id).ToList();
        }

        public int GetFirstSquadId()
        {
            return GetSquadByIndex(0).Id;
        }

        public EnemySquadConfig GetSquadByIndex(int index)
        {
            return EnemySquadConfigs[index];
        }

        public EnemySquadConfig GetRandomSquad(int seed)
        {
            return EnemySquadConfigs.Random(Math.Max(0, EnemySquadConfigs.Count - RANDOM_SQUAD_RANGE), EnemySquadConfigs.Count, seed);
        }

        public int SquadCount => EnemySquadConfigs.Count;
    }
}