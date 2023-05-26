using System.Collections.Generic;
using LegionMaster.Units.Component.Attack;

namespace LegionMaster.Units.Model
{
    public interface IUnitAttackModel
    {
        float GetDamageBuff(AttackType attackType);
        IReadOnlyCollection<AttackType> AttackTypes { get; }
        int Attack { get; }
        int AttackRangeInCells { get; }
        float AttackInterval { get; }
        float AttackSpeed { get; }
        float CriticalChance { get; }
        float CriticalDamage { get; }
    }
}