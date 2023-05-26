using System.Runtime.Serialization;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class EnergyConfig
    {
        [DataMember(Name = "EnergyMax")]
        private int _energyMax;

        [DataMember(Name = "EnergyStarting")] 
        private int _energyStarting;

        [DataMember(Name = "EnergyRecoveryPerAttack")]
        private int _recoveryPerAttack;

        [DataMember(Name = "EnergyRecoveryPerHit")]
        private int _recoveryPerHit;

        [DataMember(Name = "EnergyRecoveryPerSecond")]
        private int _recoveryPerSecond;

        [DataMember(Name = "EnergyRecoveryPerDeath")]
        private int _recoveryPerDeath;

        public int EnergyMax => _energyMax;

        public int EnergyStarting => _energyStarting;

        public int RecoveryPerAttack => _recoveryPerAttack;

        public int RecoveryPerHit => _recoveryPerHit;

        public int RecoveryPerSecond => _recoveryPerSecond;

        public int RecoveryPerDeath => _recoveryPerDeath;
    }
}