using EnemyPack.SO;

namespace EnemyPack.States
{
    public class StateCombiner : StateBase
    {
        private readonly StateBase _primaryState;
        private readonly CompositionStateBase _secondaryState;

        public StateCombiner(StateBase primaryState, CompositionStateBase secondaryState) : base(null)
        {
            _primaryState = primaryState;
            _secondaryState = secondaryState;
        }
        
        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _primaryState.Enter(state, lastState);
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