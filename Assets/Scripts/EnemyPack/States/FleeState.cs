using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;
using WorldGenerationPack;

namespace EnemyPack.States
{
    public class FleeState : StateBase
    {
        private readonly Func<StateBase> _returnState;
        private readonly FleeStateData _data;
        
        private Transform _transform;

        public FleeState(SoEnemy data, Func<StateBase> returnState) : base(data)
        {
            _data = data.GetStateData<FleeStateData>();
            _returnState = returnState;
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _transform = state.transform;
        }

        public override void Execute(EnemyLogic state)
        {
            var position = _transform.position;
            var direction = ((Vector2)position - PlayerPos).normalized;

            var separation = Vector2.zero;
            foreach (var poolObj in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == _transform.gameObject) continue;
                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            _transform.position += (Vector3)(finalDir * (state.deltaTime * _data.FleeMovementSpeed));
            
            if (InRange(state, _data.FleeDetectionRange)) return;
            
            state.SwitchState(_returnState.Invoke());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}