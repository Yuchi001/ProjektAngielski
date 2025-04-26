using System;
using System.Collections.Generic;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public abstract class RootStateBase : StateBase
    {
        protected abstract StateBase GoToState { get; }
        
        public abstract void Compose(EnemyLogic state);
        
        public override void Enter(EnemyLogic state)
        {
            state.SwitchState(GoToState);   
        }

        public sealed override void Execute(EnemyLogic state)
        {
            // IGNORE
        }

        public sealed override void Reset(EnemyLogic state)
        {
            // IGNORE
        }

        public abstract List<Type> RequiredDataTypes { get; }
    }
}