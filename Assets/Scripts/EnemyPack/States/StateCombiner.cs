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

        public override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            _primaryState.Invoke().LazyExecute(state, lazyDeltaTime);
            _secondaryState.Invoke().LazyExecute(state, lazyDeltaTime);
        }

        public override void Exit(EnemyLogic state)
        {
            _primaryState.Invoke().Exit(state);
        }
    }
}