using LegionMaster.Modifiers;
using LegionMaster.Units.Component.Ai.Navigation;
using LegionMaster.Units.Component.Animation;
using LegionMaster.Units.Model.Battle;
using UnityEngine;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class MoveToPointState : BaseState
        {
            private readonly MoveAnimationWrapper _animationWrapper;
            private readonly Vector3 _targetPosition;
            private readonly float _speed;

            private OverrideValueModifier _speedModifier;

            private INavigatable Navigatable => StateMachine._navigatable;
            
            public MoveToPointState(UnitStateMachine stateMachine, Vector3 targetPosition, float speed) : base(stateMachine)
            {
                _animationWrapper = new MoveAnimationWrapper(stateMachine._animator);
                _targetPosition = targetPosition;
                _speed = speed;
            }

            public override void OnEnterState()
            {
                _speedModifier = new OverrideValueModifier(UnitBattleModel.MOVE_SPEED_PARAMETER, _speed);
                StateMachine._owner.AddModifier(_speedModifier);
                
                _animationWrapper.PlayMoveForward();
                Navigatable.GoTo(_targetPosition);
                Navigatable.OnDestinationReached += StateMachine.SetIdleState;
            }

            public override void OnExitState()
            {
                StateMachine._owner.RemoveModifier(_speedModifier);
                _animationWrapper.PlayIdle();
                Navigatable.Stop();
                Navigatable.OnDestinationReached -= StateMachine.SetIdleState;
            }

            public override void OnTick()
            {
            }
        }
    }
}