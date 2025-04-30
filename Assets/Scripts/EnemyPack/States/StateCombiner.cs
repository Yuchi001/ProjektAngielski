using System;
using EnemyPack.SO;

namespace EnemyPack.States
{
    public class StateCombiner : StateBase
    {
        private readonly Func<StateBase> _primaryState;
        private readonly Func<CompositionStateBase> _secondaryState;

        public StateCombiner(Func<StateBase> primaryState, Func<CompositionStateBase> secondaryState) : base(null)
        {
            _primaryState = primaryState;
            _secondaryState = secondaryState;
        }
        
        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _primaryState.Invoke().Enter(state, lastState);
        }

        public override void Execute(EnemyLogic state)
        {
            _primaryState.Invoke().Execute(state);
            _secondaryState.Invoke().Execute(state);
        }

        public override void Reset(EnemyLogic state)
        {
            _primaryState.Invoke().Reset(state);
        }

        public override void Exit(EnemyLogic state)
        {
            _primaryState.Invoke().Exit(state);
        }
    }
}