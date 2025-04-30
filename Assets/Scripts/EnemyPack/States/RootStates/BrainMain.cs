using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class BrainMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeState;
        private readonly FreezeForDistanceState _freezeState;
        
        public BrainMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var meleeAttackState = new MeleeAttackState(data);
            var chaseState = new ChaseState(data, () => meleeAttackState, () => GoToState);
            _freezeState = new FreezeForDistanceState(data, () => chaseState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(MeleeAttackStateData),
            typeof(ChaseStateData),
            typeof(FreezeForDistanceStateData),
        };
    }
}