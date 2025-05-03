using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class StoneManMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeState;
        private readonly FreezeForDistanceState _freezeState;
        
        public StoneManMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var circleShootState = new CircleShootState(data);
            _freezeState = new FreezeForDistanceState(data, () => circleShootState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(CircleShootStateData),
            typeof(FreezeForDistanceStateData),
        };
    }
}