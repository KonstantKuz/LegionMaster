using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Config.Csv;
using LegionMaster.Units.Component.Attack;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class UnitConfig : ICustomCsvSerializable
    {
        public const int ZERO_VALUE = 0;
        public const int MAX_RANGE_VALUE = 10000;
        public const int MIN_RANGE_VALUE = -10000;
        public const float MAX_PERCENT_VALUE = 100;
        public const int MIN_ATTACK_DISTANCE = 1;

        [DataMember(Name = "UnitId")]
        private string _unitId;

        [DataMember(Name = "Armor")]
        private int _armor;
        
        [DataMember(Name = "MoveSpeed")]
        private int _moveSpeed;

        [DataMember(Name = "DodgeChance")]
        private float _dodgeChance;
        
        [DataMember(Name = "AbilityEnabled")]
        private bool _abilityEnabled;
        
        private Dictionary<AttackType, ResistanceConfig> _resistanceConfigs;

        [DataMember(Name = "AttackConfig")]
        private AttackConfig _attackConfig;
        
        [DataMember(Name = "HealthConfig")]
        private HealthConfig _healthConfig;

        [DataMember(Name = "RankConfig")]
        private RankConfig _rankConfig;

        [DataMember]
        private EnergyConfig _energyConfig;   
        
        public ResistanceConfig GetResistanceConfig(AttackType attackType) => _resistanceConfigs[attackType];
        public string UnitId => _unitId;
        public int Armor => Mathf.Clamp(_armor, ZERO_VALUE, MAX_RANGE_VALUE);
        public int MoveSpeed => Mathf.Clamp(_moveSpeed, ZERO_VALUE, MAX_RANGE_VALUE);
        public float DodgeChance => Mathf.Clamp(_dodgeChance, ZERO_VALUE, MAX_PERCENT_VALUE);
        public bool AbilityEnabled => _abilityEnabled;
        public AttackConfig AttackConfig => _attackConfig;      
        public HealthConfig HealthConfig => _healthConfig;
        public RankConfig RankConfig => _rankConfig;
        public EnergyConfig EnergyConfig => _energyConfig;
        
        public UnitConfig()
        {
        }

        public UnitConfig(string unitId,
            int armor, 
            HealthConfig healthConfig, 
            int moveSpeed, 
            float dodgeChance, 
            Dictionary<AttackType, ResistanceConfig> resistanceConfigs, 
            AttackConfig attackConfig, 
            RankConfig rankConfig,
            EnergyConfig energyConfig)
        {
            _unitId = unitId;
            _armor = armor;
            _healthConfig = healthConfig;
            _moveSpeed = moveSpeed;
            _dodgeChance = dodgeChance;
            _resistanceConfigs = resistanceConfigs;
            _attackConfig = attackConfig;
            _rankConfig = rankConfig;
            _energyConfig = energyConfig;
        }        

        public void Deserialize(Func<string, string> fieldValueGetter)
        {
            _resistanceConfigs = new Dictionary<AttackType, ResistanceConfig>();
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType)))
            {
                float.TryParse(fieldValueGetter(GetResistanceFieldName(attackType)), out var resistance);
                _resistanceConfigs[attackType] = new ResistanceConfig(resistance);
            }
        }

        public static string GetResistanceFieldName(AttackType attackType)
        {
            return $"{attackType.ToString()}Resistance";
        }
    }
}