using LegionMaster.Units.Component.Ability;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class AbilityState : BaseState
        {
            public AbilityState(UnitStateMachine stateMachine) : base(stateMachine)
            {
            }

            public override void OnEnterState()
            {
                Ability.StartAbility(End);
            }

            public override void OnTick()
            {
                Ability.OnTick();
            }

            public override void OnExitState()
            {
                Ability.StopAbility();
            }
            
            private void End()
            {
                StateMachine.SetState(new IdleState(StateMachine));
            }

            private BaseAbility Ability => StateMachine.Ability;
        }
    }
}