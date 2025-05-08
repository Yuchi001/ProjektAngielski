using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public class FleeState : StateBase
    {
        private readonly Func<StateBase> _returnState;
        private readonly FleeStateData _data;
        
        private Transform _transform;

        private List<EnemyLogic> _cachedNearbyEnemies = new();

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
            var minSeparationDistance = 1.5f;

            EnemyManager.GetNearbyEnemies(state.transform.position, minSeparationDistance, ref _cachedNearbyEnemies);
            foreach (var poolObj in _cachedNearbyEnemies)
            {
                if (poolObj.gameObject == state.transform.gameObject) continue;

                var pushAway = (Vector2)(state.transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;

                if (dist < minSeparationDistance)
                {
                    var bodySizeFactor = state.EnemyData.BodyScale * state.transform.localScale.x;
                    separation += pushAway.normalized * ((minSeparationDistance - dist) * bodySizeFactor);
                }
                else
                {
                    separation += pushAway.normalized * 0.05f;
                }
            }
            _cachedNearbyEnemies.Clear();

            var finalDir = (direction + separation * 0.1f).normalized;
            
            _transform.position += (Vector3)(finalDir * GetMovementSpeed(state, _data.FleeMovementSpeed));
            
            if (InRange(state, _data.FleeDetectionRange)) return;
            
            state.SwitchState(_returnState.Invoke());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}