using System.Collections.Generic;
using LegionMaster.Units.Component.Attack;
using UniRx;

namespace LegionMaster.Units.Model
{
    public interface IUnitModel
    {
        string UnitId { get; }
        int Armor { get; }
        IReadOnlyReactiveProperty<int> MoveSpeed { get; }
        float DodgeChance { get; }
        IUnitAttackModel UnitAttack { get; }
        IUnitHealthModel UnitHealth { get; }
        IUnitEnergyModel UnitEnergy { get; }
        IEnumerable<string> Factions { get; }
        float GetResistance(AttackType attackType);
        bool AbilityEnabled { get; }
        int Star { get; }
        
        bool HudVisible { get; set; }
        bool AiEnabled { get; set; }
        bool StarBarVisible { get; set; }

    }
}