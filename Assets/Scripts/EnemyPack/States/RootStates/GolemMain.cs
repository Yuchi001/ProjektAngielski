using System;
using System.Collections.Generic;

namespace EnemyPack.States.RootStates
{
    public class GolemMain : RootStateBase
    {
        protected override StateBase GoToState => _chaseState;

        private ChaseState _chaseState;
        
        public override void Compose(EnemyLogic logic)
        {
            _chaseState = new ChaseState();
        }

        public override List<Type> RequiredDataTypes => new();
    }
}