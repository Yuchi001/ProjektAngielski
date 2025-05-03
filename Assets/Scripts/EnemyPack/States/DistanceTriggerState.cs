using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;
using Utils;

namespace EnemyPack.States
{
    public class DistanceTriggerState : CompositionStateBase
    {
        private readonly DistanceTriggerStateData _data;
        private readonly Func<StateBase> _nextState;

        public DistanceTriggerState(SoEnemy _enemyData, Func<StateBase> nextState) : base(_enemyData)
        {
            _data = _enemyData.GetStateData<DistanceTriggerStateData>();
            _nextState = nextState;
        }
        
        public override void Execute(EnemyLogic state)
        {
            var inRange = state.transform.InRange(PlayerPos, _data.TriggerDistance);
            if (inRange && _data.TriggerType == DistanceTriggerStateData.ETriggerType.ENTER) state.SwitchState(_nextState.Invoke());
            if (!inRange && _data.TriggerType == DistanceTriggerStateData.ETriggerType.EXIT) state.SwitchState(_nextState.Invoke());
        }
    }
}