using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public class BatMain : RootStateBase
    {
        protected override StateBase GoToState => _chaseState;
        private readonly ChaseState _chaseState;
        
        public BatMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var meleeAttackState = new MeleeAttackState(data);
            _chaseState = new ChaseState(data, meleeAttackState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(MeleeAttackStateData),
            typeof(ChaseStateData),
        };
    }
}