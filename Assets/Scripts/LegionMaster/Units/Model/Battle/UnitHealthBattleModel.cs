using LegionMaster.Modifiers;

namespace LegionMaster.Units.Model.Battle
{
    public class UnitHealthBattleModel: IUnitHealthModel
    {
        public const string STARTING_HEALTH_PARAMETER = "StartingHealth";
        public const string MAX_HEALTH_PARAMETER = "MaxHealth";

        private readonly FloatModifiableParameter _startingHealth;    
        private readonly FloatModifiableParameter _maxHealth;
        private readonly FloatModifiableParameter _recoveryPerAttack;
        private readonly FloatModifiableParameter _recoveryPerHit;
        private readonly FloatModifiableParameter _recoveryPerSecond;
        private readonly FloatModifiableParameter _recoveryPerDeath;
        
        public UnitHealthBattleModel(IUnitHealthModel unitHealth, IModifiableParameterOwner parameterOwner)
        {
            _startingHealth = new FloatModifiableParameter(STARTING_HEALTH_PARAMETER, unitHealth.StartingHealth, parameterOwner);
            _maxHealth = new FloatModifiableParameter(MAX_HEALTH_PARAMETER, unitHealth.MaxHealth, parameterOwner);
            _recoveryPerAttack = new FloatModifiableParameter("HealthRecoveryPerAttack", unitHealth.RecoveryPerAttack,
                parameterOwner);
            _recoveryPerHit = new FloatModifiableParameter("HealthRecoveryPerHit", unitHealth.RecoveryPerHit,
                parameterOwner);
            _recoveryPerSecond = new FloatModifiableParameter("HealthRecoveryPerSecond", unitHealth.RecoveryPerSecond,
                parameterOwner);
            _recoveryPerDeath = new FloatModifiableParameter("HealthRecoveryPerDeath", unitHealth.RecoveryPerDeath,
                parameterOwner);
        }

        public int MaxHealth => (int)_maxHealth.Value;
        public int StartingHealth => (int)_startingHealth.Value;
        public int RecoveryPerAttack => (int)_recoveryPerAttack.Value;
        public int RecoveryPerHit => (int)_recoveryPerHit.Value;
        public int RecoveryPerSecond => (int)_recoveryPerSecond.Value;
        public int RecoveryPerDeath => (int)_recoveryPerDeath.Value;
    }
}