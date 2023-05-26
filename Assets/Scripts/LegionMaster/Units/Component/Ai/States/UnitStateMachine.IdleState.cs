using System.Linq;
using JetBrains.Annotations;
using LegionMaster.NavMap.Model;
using LegionMaster.Units.Component.Animation;
using LegionMaster.Units.Component.Target;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class IdleState : BaseState
        {
            private readonly MoveAnimationWrapper _animationWrapper;
            
            public IdleState(UnitStateMachine stateMachine) : base(stateMachine)
            {
                _animationWrapper = new MoveAnimationWrapper(stateMachine._animator);
            }            

            public override void OnEnterState()
            {
                StateMachine._navigatable.Stop();
                _animationWrapper.PlayIdle();
            }

            public override void OnExitState()
            {
            }

            public override void OnTick()
            {
                if (StateMachine.AbilityStart())
                {
                    return;
                }
                UpdateTarget();
            }
            
            private void UpdateTarget()
            {
                var targetSearchResult = StateMachine._navMapService.FindPath(StateMachine.CurrentCellId, StateMachine.EnemyUnitType, StateMachine.AttackRangeInCells);

                if (targetSearchResult.TargetCell == null)
                {
                    ClearTarget();
                    return;
                }
            
                StateMachine.Target = GetTargetByLastSearchResult(targetSearchResult);
                if (StateMachine.IsTargetInvalid)
                {
                    ClearTarget();
                    return;
                }
                
                if (targetSearchResult.CanAttackFromStartingPosition)
                {
                    StateMachine.SetState(new AttackState(StateMachine));
                }
                else
                {
                    StateMachine.SetState(new MoveToCellState(StateMachine, targetSearchResult.Path.First()));
                }
            }    
            
            private void ClearTarget()
            {
                StateMachine.Target = null;
            }   
            
            private ITarget GetTargetByLastSearchResult([CanBeNull] TargetSearchResult targetSearchResult)
            {
                return targetSearchResult?.TargetCell == null ? null : StateMachine._targetListProvider.FindById(targetSearchResult.TargetCell.UnitId);
            }            
        }
    }
}