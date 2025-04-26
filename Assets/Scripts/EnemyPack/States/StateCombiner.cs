namespace EnemyPack.States
{
    public class StateCombiner : StateBase
    {
        private readonly StateBase _primaryState;
        private readonly StateBase _secondaryState;

        public StateCombiner(StateBase primaryState, StateBase secondaryState)
        {
            _primaryState = primaryState;
            _secondaryState = secondaryState;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _primaryState.Enter(state);
        }

        public override void Execute(EnemyLogic state)
        {
            _primaryState.Execute(state);
            _secondaryState.Execute(state);
        }

        public override void Reset(EnemyLogic state)
        {
            _primaryState.Reset(state);
        }
    }
}