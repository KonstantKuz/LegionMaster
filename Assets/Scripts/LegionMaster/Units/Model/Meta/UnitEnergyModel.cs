using LegionMaster.Units.Config;

namespace LegionMaster.Units.Model.Meta
{
    public class UnitEnergyModel: IUnitEnergyModel
    {
        private readonly EnergyConfig _energyConfig;

        public UnitEnergyModel(EnergyConfig energyConfig)
        {
            _energyConfig = energyConfig;
        }

        public int MaxEnergy => _energyConfig.EnergyMax;
        public int StartingEnergy => _energyConfig.EnergyStarting;
        public int RecoveryPerAttack => _energyConfig.RecoveryPerAttack;
        public int RecoveryPerHit => _energyConfig.RecoveryPerHit;
        public int RecoveryPerSecond => _energyConfig.RecoveryPerSecond;
        public int RecoveryPerDeath => _energyConfig.RecoveryPerDeath;
    }
}