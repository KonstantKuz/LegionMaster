using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Config.Csv;
using LegionMaster.Units.Component.Attack;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class AttackConfig : ICustomCsvSerializable
    {
        [DataMember(Name = "Attack")]
        private int _attack;

        [DataMember(Name = "AttackRangeInCells")]
        private int _attackRangeInCells;

        [DataMember(Name = "AttackInterval")]
        private float _attackInterval;

        [DataMember(Name = "CriticalChance")]
        private float _criticalChance;

        [DataMember(Name = "CriticalDamage")]
        private float _criticalDamage;
        
        [DataMember(Name = "AttackTypes")]
        private AttackType[] _attackTypes;
        
        private Dictionary<AttackType, DamageBuffConfig> _damageBuffConfigs;
        public DamageBuffConfig GetDamageBuffConfig(AttackType attackType) => _damageBuffConfigs[attackType];
        public IReadOnlyCollection<AttackType> AttackTypes => _attackTypes;
        public int Attack => Mathf.Clamp(_attack, UnitConfig.ZERO_VALUE, UnitConfig.MAX_RANGE_VALUE);
        public int AttackRangeInCells => Mathf.Clamp(_attackRangeInCells, UnitConfig.MIN_ATTACK_DISTANCE, UnitConfig.MAX_RANGE_VALUE);
        public float AttackInterval => Mathf.Clamp(_attackInterval, UnitConfig.ZERO_VALUE, UnitConfig.MAX_RANGE_VALUE);
        public float AttackSpeed => 1 / AttackInterval;
        public float CriticalChance => Mathf.Clamp(_criticalChance, UnitConfig.ZERO_VALUE, UnitConfig.MAX_PERCENT_VALUE);
        public float CriticalDamage => Mathf.Clamp(_criticalDamage, UnitConfig.ZERO_VALUE, UnitConfig.MAX_RANGE_VALUE);
        
        public AttackConfig()
        {
        }

        public AttackConfig(int attack, int attackRangeInCells, float attackInterval, float criticalChance, float criticalDamage, AttackType[] attackTypes, Dictionary<AttackType, DamageBuffConfig> damageBuffConfigs)
        {
            _attack = attack;
            _attackRangeInCells = attackRangeInCells;
            _attackInterval = attackInterval;
            _criticalChance = criticalChance;
            _criticalDamage = criticalDamage;
            _attackTypes = attackTypes;
            _damageBuffConfigs = damageBuffConfigs;
        }        

        public void Deserialize(Func<string, string> fieldValueGetter)
        {
            DeserializeDamageBuffConfigs(fieldValueGetter);
        }
        private void DeserializeDamageBuffConfigs(Func<string, string> fieldValueGetter)
        {
            _damageBuffConfigs = new Dictionary<AttackType, DamageBuffConfig>();
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType)))
            {
                float.TryParse(fieldValueGetter(GetDamageBuffFieldName(attackType)), out var damageBuff);
                _damageBuffConfigs[attackType] = new DamageBuffConfig(damageBuff);
            }
        }

        public static string GetDamageBuffFieldName(AttackType attackType)
        {
            return $"{attackType.ToString()}DamageBuff";
        }
    }
}