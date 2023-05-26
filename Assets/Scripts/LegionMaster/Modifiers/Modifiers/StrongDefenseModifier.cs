using LegionMaster.Units.Model.Battle;

namespace LegionMaster.Modifiers
{
    public class StrongDefenseModifier: IModifier
    {
        private readonly float _percent;

        public StrongDefenseModifier(float percent)
        {
            _percent = percent;
        }

        public void Apply(IModifiableParameterOwner owner)
        {
            //this add dependency to unit model. Which is bad. But I do no want at the moment to add more parameters to modifiers config            
            var attackParam = owner.GetParameter<FloatModifiableParameter>(UnitAttackBattleModel.ATTACK_PARAMETER);
            var delta = attackParam.InitialValue * _percent / 100;
            attackParam.AddValue(-delta);

            var healthParam = owner.GetParameter<FloatModifiableParameter>(UnitHealthBattleModel.STARTING_HEALTH_PARAMETER);
            healthParam.AddValue(delta);
        }
    }
}