using System.Runtime.Serialization;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class HealthConfig
    {
        [DataMember(Name = "Health")]
        private int _health;

        [DataMember(Name = "HealthRecoveryPerAttack")]
        private int _recoveryPerAttack;

        [DataMember(Name = "HealthRecoveryPerHit")]
        private int _recoveryPerHit;

        [DataMember(Name = "HealthRecoveryPerSecond")]
        private int _recoveryPerSecond;

        [DataMember(Name = "HealthRecoveryPerDeath")]
        private int _recoveryPerDeath;

        public int Health => Clamp(_health);

        public int RecoveryPerAttack => Clamp(_recoveryPerAttack);

        public int RecoveryPerHit => Clamp(_recoveryPerHit);

        public int RecoveryPerSecond => Clamp(_recoveryPerSecond);

        public int RecoveryPerDeath => Clamp(_recoveryPerDeath);

        private int Clamp(int value) => Mathf.Clamp(value, UnitConfig.ZERO_VALUE, UnitConfig.MAX_RANGE_VALUE);
    }
}