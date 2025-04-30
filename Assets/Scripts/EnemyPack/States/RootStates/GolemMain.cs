using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class GolemMain : RootStateBase
    {
        protected override StateBase GoToState => _chaseState;
        private readonly ChaseState _chaseState;
        
        public GolemMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var attackState = new MeleeAttackState(data);
            _chaseState = new ChaseState(data, () => attackState);
        }
       
        public override List<Type> RequiredDataTypes => new()
        {
			typeof(MeleeAttackStateData),
			typeof(ChaseStateData),
        };
    }
}