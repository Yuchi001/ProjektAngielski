using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public class MimicMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeState;
        private readonly FreezeForDistanceState _freezeState;
        
        public MimicMain(SoEnemy data) : base(data)
        {
            if (data == null) return;

            var attackState = new MeleeAttackState(data);
            var chaseState = new ChaseState(data, attackState);
            _freezeState = new FreezeForDistanceState(data, chaseState);
        }

        public override ESpriteRotation GetRotation(EnemyLogic state)
        {
            return ESpriteRotation.None;
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(MeleeAttackStateData),
            typeof(ChaseStateData),
            typeof(FreezeForDistanceStateData),
        };
    }
}