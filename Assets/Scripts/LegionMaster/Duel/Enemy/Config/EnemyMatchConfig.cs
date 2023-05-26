using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Enemy.Config;

namespace LegionMaster.Duel.Enemy.Config
{
    public class EnemyMatchConfig
    {
        public readonly int Match;
        public readonly IReadOnlyList<EnemySquadConfig> EnemySquadRounds;

        public EnemyMatchConfig(int match, IEnumerable<EnemySpawnRoundConfig> enemySpawns)
        {
            Match = match;
            EnemySquadRounds = enemySpawns
                               .GroupBy(it => it.Round)
                               .Select(it => new EnemySquadConfig(it.Key, 
                                                                  it.Select(s => s.EnemySpawn)))
                               .ToList();
        }
        public EnemySquadConfig GetEnemySquad(int round)
        {
            return EnemySquadRounds.FirstOrDefault(it => it.Id == round) ?? throw new NullReferenceException($"EnemySquadConfig is null, round:= {round}");
        }

        public EnemySquadConfig GetMaxRoundSquad() => EnemySquadRounds.Last();

        public int RoundCount => EnemySquadRounds.Count;
    }
}