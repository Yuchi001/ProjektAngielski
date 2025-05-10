using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;

namespace EnemyPack.States.RootStates
{
    public class DummyMain : RootStateBase
    {
        protected override StateBase GoToState => _freezeState;
        private readonly FreezeState _freezeState;
        
        public DummyMain(SoEnemy data) : base(data)
        {
            if (data == null) return;
            
            _freezeState = new FreezeState(data);
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(FreezeStateData),
        };
    }
}