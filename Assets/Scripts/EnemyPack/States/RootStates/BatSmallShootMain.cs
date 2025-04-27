using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public class BatSmallShootMain : RootStateBase
    {
        protected override StateBase GoToState => _combinedPatrolState;
        private readonly StateCombiner _combinedPatrolState;
        
        public BatSmallShootMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var shootState = new ShootState(data);
            var patrolState = new PatrolState(data, shootState);
            var fleeState = new FleeState(data, patrolState);
            var distanceTriggerState = new DistanceTriggerState(data, fleeState);
            _combinedPatrolState = new StateCombiner(patrolState, distanceTriggerState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(ShootStateData),
            typeof(PatrolStateData),
            typeof(FleeStateData),
            typeof(DistanceTriggerStateData),
        };
    }
}