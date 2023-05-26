namespace LegionMaster.Units.Model
{
    public interface IUnitEnergyModel
    {
        int MaxEnergy { get; }
        int StartingEnergy { get; }
        int RecoveryPerAttack { get; }
        int RecoveryPerHit { get; }
        int RecoveryPerSecond { get; }
        int RecoveryPerDeath { get; }
    }
}