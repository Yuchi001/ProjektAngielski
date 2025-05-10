using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public abstract class RootStateBase : StateBase
    {
        protected abstract StateBase GoToState { get; }
        
        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            state.SwitchState(GoToState);   
        }

        public sealed override void Execute(EnemyLogic state)
        {
            // IGNORE
        }

        public sealed override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            // IGNORE
        }

        public abstract List<Type> RequiredDataTypes { get; }

        protected RootStateBase(SoEnemy data) : base(data)
        {
        }
    }
}