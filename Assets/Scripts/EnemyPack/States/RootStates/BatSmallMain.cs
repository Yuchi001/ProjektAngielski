using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class BatSmallMain : RootStateBase
    {
        protected override StateBase GoToState => _combinedPatrolState;
        private readonly StateCombiner _combinedPatrolState;
        
        public BatSmallMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var patrolState = new PatrolState(data);
            var meleeAttackState = new MeleeAttackState(data);
            var chaseState = new ChaseState(data, () => meleeAttackState, () => patrolState);
            var triggerState = new DistanceTriggerState(data, () => chaseState);
            _combinedPatrolState = new StateCombiner(() =>patrolState, () =>triggerState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(PatrolStateData),
            typeof(MeleeAttackStateData),
            typeof(ChaseStateData),
            typeof(DistanceTriggerStateData),
        };
    }
}