using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public sealed class MonumentMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeState;
        private readonly FreezeForDistanceState _freezeState;
        
        public MonumentMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var shootingState = new ShootState(data);
            _freezeState = new FreezeForDistanceState(data, () => shootingState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(ShootStateData),
            typeof(FreezeForDistanceStateData),
        };
    }
}