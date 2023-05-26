using LegionMaster.Location.GridArena.Model;
using LegionMaster.Units.Component.Ai.Navigation;
using LegionMaster.Units.Component.Animation;
using UnityEngine.Assertions;

namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class MoveToCellState : BaseState
        {
            private readonly MoveAnimationWrapper _animationWrapper;
            private readonly CellId _destinationCellId;
            
            public MoveToCellState(UnitStateMachine stateMachine, CellId destinationCellId) : base(stateMachine)
            {
                Assert.AreNotEqual(destinationCellId, CellId.InvalidCellId);
                _animationWrapper = new MoveAnimationWrapper(stateMachine._animator);
                _destinationCellId = destinationCellId;
            }

            public override void OnEnterState()
            {
                var targetPos = StateMachine._gridPositionProvider.GetCellPos(_destinationCellId);
                Navigatable.GoTo(targetPos);
                ChangeCellTo(_destinationCellId);  //Это некорректно. Географически наш юнит все еще находиться в старой клетке
                //и только еще начинает разворачиваться. А с точки зрения pathfinding-а - он уже в новой клетке. 
                //Юниты противника могут при расчете дистанции атаки могут решить, что им пора его догонять, хотя визуально это не так.
                //Но нам надо зарезервировать эту клетку за нами сейчас, иначе ее могут занять другие юниты.
                //Если этот баг всплывет, то надо будет добавить состояние резервации клетки за юнитом.
                Navigatable.OnDestinationReached += OnDestinationReached;
                _animationWrapper.PlayMoveForward();
            }
            public override void OnExitState()
            {
                Navigatable.Stop();
                Navigatable.OnDestinationReached -= OnDestinationReached;
                _animationWrapper.PlayIdle();
            }

            public override void OnTick()
            {
            }

            private void OnDestinationReached()
            {
                StateMachine.SetState(new IdleState(StateMachine));
            }
            
            private void ChangeCellTo(CellId cellId)
            {
                StateMachine._navMapService.MoveUnit(StateMachine.CurrentCellId, cellId, StateMachine._unitType, StateMachine._selfTarget.TargetId);
                StateMachine.CurrentCellId = cellId;            
            }
            private INavigatable Navigatable => StateMachine._navigatable;
        }
    }
}