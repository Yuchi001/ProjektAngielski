using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class CrabMain : RootStateBase
    {
        protected override StateBase GoToState => _runAndLook;
        private readonly StateCombiner _runAndLook;
        
        public CrabMain(SoEnemy data) : base(data)
        {
            if (data == null) return;

            var freezeState = new FreezeState(data);
            var meleeAttackState = new MeleeAttackState(data);
            var chaseState = new ChaseState(data, () => meleeAttackState);
            var lookAwayState = new LookAtTriggerState(data, () => GoToState, LookAtTriggerState.ELookType.LOOK_AWAY);
            var combinedLookAwayState = new StateCombiner(() => freezeState, () => lookAwayState);
            var lookAtState = new LookAtTriggerState(data, () => combinedLookAwayState, LookAtTriggerState.ELookType.LOOK_AT);
            _runAndLook = new StateCombiner(() => chaseState, () => lookAtState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(FreezeStateData),
            typeof(MeleeAttackStateData),
            typeof(ChaseStateData),
        };
    }
}