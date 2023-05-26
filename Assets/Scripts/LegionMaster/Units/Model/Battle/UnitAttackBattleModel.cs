using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Modifiers;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;

namespace LegionMaster.Units.Model.Battle
{
    public class UnitAttackBattleModel: IUnitAttackModel
    {
        public const string ATTACK_PARAMETER = "Attack";
        public const string ATTACK_RANGE_PARAMETER = "AttackRange";
        public const string ATTACK_INTERVAL_PARAMETER = "AttackInterval";
        public const string CRITICAL_CHANCE_PARAMETER = "CriticalChance";
        public const string CRITICAL_DAMAGE_PARAMETER = "CriticalDamage";
        public const string ATTACK_TYPE_PARAMETER = "AttackType";
        
        private readonly FloatModifiableParameter _attack;
        private readonly FloatModifiableParameter _range;
        private readonly FloatModifiableParameter _interval;
        private readonly FloatModifiableParameter _criticalChance;
        private readonly FloatModifiableParameter _criticalDamage;
        private readonly Dictionary<AttackType, FloatModifiableParameter> _damageBuffs;
        private readonly CollectionParameter<AttackType> _attackTypes;
        
        public UnitAttackBattleModel(IUnitAttackModel unitAttack, IModifiableParameterOwner parameterOwner)
        {
            _attack = new FloatModifiableParameter(ATTACK_PARAMETER, unitAttack.Attack, parameterOwner);
            _range = new FloatModifiableParameter(ATTACK_RANGE_PARAMETER, unitAttack.AttackRangeInCells, parameterOwner);
            _interval = new FloatModifiableParameter(ATTACK_INTERVAL_PARAMETER, unitAttack.AttackInterval, parameterOwner);
            _criticalChance = new FloatModifiableParameter(CRITICAL_CHANCE_PARAMETER, unitAttack.CriticalChance, parameterOwner);
            _criticalDamage = new FloatModifiableParameter(CRITICAL_DAMAGE_PARAMETER, unitAttack.CriticalDamage, parameterOwner);
            _damageBuffs = EnumExt.Values<AttackType>().ToDictionary(
                it => it, 
                it => new FloatModifiableParameter(AttackConfig.GetDamageBuffFieldName(it), unitAttack.GetDamageBuff(it), parameterOwner));
            _attackTypes =
                new CollectionParameter<AttackType>(ATTACK_TYPE_PARAMETER, unitAttack.AttackTypes, parameterOwner, true);
        }

        public float GetDamageBuff(AttackType attackType) => _damageBuffs[attackType].Value;

        public IReadOnlyCollection<AttackType> AttackTypes => _attackTypes.Value;
        public int Attack => (int)_attack.Value;
        public int AttackRangeInCells => (int)_range.Value;
        public float AttackInterval => (int)_interval.Value;
        public float AttackSpeed => 1.0f / _interval.Value;
        public float CriticalChance => _criticalChance.Value;
        public float CriticalDamage => _criticalDamage.Value;
    }
}