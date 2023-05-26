using System.Runtime.Serialization;
using LegionMaster.Enemy.Config;

namespace LegionMaster.Duel.Enemy.Config
{
    [DataContract]
    public class EnemySpawnRoundConfig
    {
        [DataMember(Name = "Round")]
        public int Round;
        [DataMember]
        public EnemySpawnConfig EnemySpawn;
    }
}