using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public class FreezeForTimeState : FreezeState
    {
        private readonly FreezeForTimeStateData _data;
        private readonly Func<StateBase> _nextState;

        private float _timer = 0;
        
        public FreezeForTimeState(SoEnemy data, Func<StateBase> nextState) : base(data, data.GetStateData<FreezeForTimeStateData>())
        {
            _data = data.GetStateData<FreezeForTimeStateData>();
            _nextState = nextState;
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            base.Enter(state, lastState);
            _timer = 0;
        }

        public override void Execute(EnemyLogic state)
        {
            base.Execute(state);
            
            _timer += Time.deltaTime;
            if (_timer < _data.FreezeTime) return;

            state.SwitchState(_nextState.Invoke());
        }
    }
}