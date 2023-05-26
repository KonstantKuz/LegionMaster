namespace LegionMaster.Units.Model
{
    public interface IUnitHealthModel
    {
        int MaxHealth { get; }
        int StartingHealth { get; }
        int RecoveryPerAttack { get; }
        int RecoveryPerHit { get; }
        int RecoveryPerSecond { get; }
        int RecoveryPerDeath { get; }
    }
}