namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private class DeathState : BaseState
        {
            public DeathState(UnitStateMachine stateMachine) : base(stateMachine)
            {
            }

            public override void OnEnterState()
            {
                StateMachine._navigatable.Stop();
                StateMachine._deathAnimation.PlayDeath();
                RemoveUnitFromNavMap();
            }

            public override void OnTick()
            {
            }

            public override void OnExitState()
            {
            }
            
            private void RemoveUnitFromNavMap()
            {
                StateMachine._navMapService.RemoveUnitFromCell(StateMachine.CurrentCellId, StateMachine._selfTarget.TargetId);
            }            
        }
    }
}