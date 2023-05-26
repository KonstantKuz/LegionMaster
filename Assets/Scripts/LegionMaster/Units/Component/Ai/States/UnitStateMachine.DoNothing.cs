using LegionMaster.Units.Component.Animation;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class DoNothingState : BaseState
        {
            private readonly MoveAnimationWrapper _animationWrapper;
            public DoNothingState(UnitStateMachine stateMachine) : base(stateMachine)
            {
                _animationWrapper = new MoveAnimationWrapper(stateMachine._animator);
            }

            public override void OnEnterState()
            {
                _animationWrapper.PlayIdle();
            }

            public override void OnTick()
            {
            }

            public override void OnExitState()
            {
            }
        }
    }
}