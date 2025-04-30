using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public class FreezeForDistanceState : FreezeState
    {
        private readonly FreezeForDistanceStateData _data;
        private readonly Func<StateBase> _nextState;
        
        public FreezeForDistanceState(SoEnemy data, Func<StateBase> nextState) : base(data, data.GetStateData<FreezeForDistanceStateData>())
        {
            _data = data.GetStateData<FreezeForDistanceStateData>();
            _nextState = nextState;
        }

        public override void Execute(EnemyLogic state)
        {
            base.Execute(state);
            if (!InRange(state, _data.FreezeDetectionRange)) return;

            SwitchToNextState(state, _nextState.Invoke());
        }
    }
}