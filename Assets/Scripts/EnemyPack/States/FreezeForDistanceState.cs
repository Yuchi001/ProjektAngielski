using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public class FreezeForDistanceState : FreezeState
    {
        private readonly FreezeForDistanceStateData _data;
        private readonly StateBase _nextState;
        
        public FreezeForDistanceState(SoEnemy data, StateBase nextState) : base(data, data.GetStateData<FreezeForDistanceStateData>().IsInvincibleDuringFreeze)
        {
            _data = data.GetStateData<FreezeForDistanceStateData>();
            _nextState = nextState;
        }

        public override void Execute(EnemyLogic state)
        {
            if (!InRange(state, _data.FreezeDetectionRange)) return;

            SwitchToNextState(state, _nextState);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}