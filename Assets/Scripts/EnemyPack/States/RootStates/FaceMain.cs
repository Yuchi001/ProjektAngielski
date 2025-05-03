using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public sealed class FaceMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeForTime;
        private readonly FreezeForTimeState _freezeForTime;
        
        public FaceMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            var chargeState = new ChargeState(data, () => _freezeForTime);
            _freezeForTime = new FreezeForTimeState(data, () => chargeState);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(ChargeStateData),
            typeof(FreezeForTimeStateData),
        };
    }
}