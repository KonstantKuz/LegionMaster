using LegionMaster.Modifiers;

namespace LegionMaster.Units.Model.Battle
{
    public class UnitEnergyBattleModel: IUnitEnergyModel
    {
        public const string STARTING_ENERGY_PARAMETER = "StartingEnergy";
        public const string MAX_ENERGY_PARAMETER = "MaxEnergy";

        private readonly FloatModifiableParameter _startingEnergy;    
        private readonly FloatModifiableParameter _maxEnergy;
        private readonly FloatModifiableParameter _recoveryPerAttack;
        private readonly FloatModifiableParameter _recoveryPerHit;
        private readonly FloatModifiableParameter _recoveryPerSecond;
        private readonly FloatModifiableParameter _recoveryPerDeath;
        
        public UnitEnergyBattleModel(IUnitEnergyModel unitEnergy, IModifiableParameterOwner parameterOwner)
        {
            _startingEnergy = new FloatModifiableParameter(STARTING_ENERGY_PARAMETER, unitEnergy.StartingEnergy, parameterOwner);
            _maxEnergy = new FloatModifiableParameter(MAX_ENERGY_PARAMETER, unitEnergy.MaxEnergy, parameterOwner);
            _recoveryPerAttack = new FloatModifiableParameter("EnergyRecoveryPerAttack", unitEnergy.RecoveryPerAttack,
                parameterOwner);
            _recoveryPerHit = new FloatModifiableParameter("EnergyRecoveryPerHit", unitEnergy.RecoveryPerHit,
                parameterOwner);
            _recoveryPerSecond = new FloatModifiableParameter("EnergyRecoveryPerSecond", unitEnergy.RecoveryPerSecond,
                parameterOwner);
            _recoveryPerDeath = new FloatModifiableParameter("EnergyRecoveryPerDeath", unitEnergy.RecoveryPerDeath,
                parameterOwner);
        }

        public int MaxEnergy => (int)_maxEnergy.Value;
        public int StartingEnergy => (int)_startingEnergy.Value;
        public int RecoveryPerAttack => (int)_recoveryPerAttack.Value;
        public int RecoveryPerHit => (int)_recoveryPerHit.Value;
        public int RecoveryPerSecond => (int)_recoveryPerSecond.Value;
        public int RecoveryPerDeath => (int)_recoveryPerDeath.Value;
    }
}