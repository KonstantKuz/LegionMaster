using System;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Model.Battle;

namespace LegionMaster.Modifiers
{
    public class AddAttackTypeModifier: IModifier
    {
        private readonly AttackType _attackType;

        public AddAttackTypeModifier(string attackType)
        {
            _attackType = (AttackType)Enum.Parse(typeof(AttackType), attackType);
        }

        public void Apply(IModifiableParameterOwner parameterOwner)
        {
            var param = parameterOwner.GetParameter<CollectionParameter<AttackType>>(UnitAttackBattleModel.ATTACK_TYPE_PARAMETER);
            param.AddValue(_attackType);
        }
    }
}