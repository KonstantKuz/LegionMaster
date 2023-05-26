using System.Collections.Generic;
using System.Linq;

namespace LegionMaster.Enemy.Config
{
    public class EnemySquadConfig
    {
        public readonly int Id;
        public readonly IReadOnlyList<EnemySpawnConfig> EnemySpawns;

        public EnemySquadConfig(int id, IEnumerable<EnemySpawnConfig> enemySpawns)
        {
            Id = id;
            EnemySpawns = enemySpawns.ToList();
        }
    }
}