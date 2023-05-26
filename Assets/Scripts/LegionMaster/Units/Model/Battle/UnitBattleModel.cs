using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Modifiers;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model.Meta;
using UniRx;

namespace LegionMaster.Units.Model.Battle
{
    public class UnitBattleModel : IUnitModel, IModifiableParameterOwner
    {
        public const string ARMOR_PARAMETER = "Armor";
        public const string MOVE_SPEED_PARAMETER = "MoveSpeed";
        public const string DODGE_CHANGE_PARAMETER = "DodgeChance";
        
        private readonly ICollection<IModifier> _modifiers = new HashSet<IModifier>();
        private readonly Dictionary<string, IModifiableParameter> _parameters = new Dictionary<string, IModifiableParameter>();

        private readonly FloatModifiableParameter _armor;
        private readonly FloatModifiableParameter _moveSpeed;
        private readonly FloatModifiableParameter _dodgeChange;
        private readonly Dictionary<AttackType, FloatModifiableParameter> _resistance;

        private readonly UnitModel _base;
        
        public UnitBattleModel(UnitModel unitModel)
        {
            _base = unitModel;
            HudVisible = true;
            AiEnabled = true;
            
            _armor = new FloatModifiableParameter(ARMOR_PARAMETER, unitModel.Armor, this);
            _moveSpeed = new FloatModifiableParameter(MOVE_SPEED_PARAMETER, unitModel.MoveSpeed.Value, this);
            _dodgeChange = new FloatModifiableParameter(DODGE_CHANGE_PARAMETER, unitModel.DodgeChance, this);
            _resistance = EnumExt.Values<AttackType>().ToDictionary(type => type, 
                                                                    type => new FloatModifiableParameter(UnitConfig.GetResistanceFieldName(type), unitModel.GetResistance(type), this, float.NegativeInfinity));
            
            UnitAttack = new UnitAttackBattleModel(unitModel.UnitAttack, this);
            UnitHealth = new UnitHealthBattleModel(unitModel.UnitHealth, this);
            UnitEnergy = new UnitEnergyBattleModel(unitModel.UnitEnergy, this);
        }

        public string UnitId => _base.UnitId;
        public int Armor => (int) _armor.Value;
        public IReadOnlyReactiveProperty<int> MoveSpeed => _moveSpeed.ReactiveValue.Select(it => (int) it).ToReactiveProperty();
        public float DodgeChance => _dodgeChange.Value;
        public IEnumerable<string> Factions => _base.Factions;
        public bool AbilityEnabled => _base.AbilityEnabled;
        
        public int Star => _base.RankedUnit.Star;
        
        public IUnitAttackModel UnitAttack { get; }
        public IUnitHealthModel UnitHealth { get; }
        public IUnitEnergyModel UnitEnergy { get; }
        
        public bool HudVisible { get; set; }

        public bool AiEnabled { get; set; }

        public bool StarBarVisible { get; set; }
        public float GetResistance(AttackType attackType) => _resistance[attackType].Value;
        public void AddModifier(IModifier modifier)
        {
            _modifiers.Add(modifier);
            modifier.Apply(this);
        }

        public IModifiableParameter GetParameter(string name)
        {
            if (!_parameters.ContainsKey(name))
            {
                throw new Exception($"No modifiable parameter named {name}");
            }
            return _parameters[name];
        }

        private void ResetAllParameters()
        {
            foreach (var parameter in _parameters.Values)
            {
                parameter.Reset();
            }
        }

        public void AddParameter(IModifiableParameter parameter)
        {
            if (_parameters.ContainsKey(parameter.Name))
            {
                throw new Exception($"UnitModel already have parameter named {parameter.Name}");
            }
            _parameters.Add(parameter.Name, parameter);
        }

        public void RemoveModifier(IModifier modifier)
        {
            _modifiers.Remove(modifier);
            
            ResetAllParameters();
            ApplyAllModifiers();
        }

        private void ApplyAllModifiers()
        {
            foreach (var modifier in _modifiers)
            {
                modifier.Apply(this);
            }
        }

      


    }
}