namespace LegionMaster.Units.Component.Ai
{
    public partial class UnitStateMachine
    {
        private abstract class BaseState
        {
            protected readonly UnitStateMachine StateMachine;

            protected BaseState(UnitStateMachine stateMachine)
            {
                StateMachine = stateMachine;
            }

            public abstract void OnEnterState();
            public abstract void OnTick();
            public abstract void OnExitState();
        }
    }
}