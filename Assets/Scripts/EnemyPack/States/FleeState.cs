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
        private Vector2 _separation;

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

            var finalDir = (direction + _separation * 0.1f).normalized;

            var ms = _data.FleeMovementSpeed * SlowModificator(state) * Time.deltaTime;
            _transform.position += (Vector3)(finalDir * ms);
        }

        public override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            _separation = Vector2.zero;
            const float minSeparationDistance = 1.5f;

            EnemyManager.GetNearbyEnemies(state.transform.position, minSeparationDistance, ref _cachedNearbyEnemies);
            foreach (var poolObj in _cachedNearbyEnemies)
            {
                if (poolObj.gameObject == state.transform.gameObject) continue;

                var pushAway = (Vector2)(state.transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;

                if (dist < minSeparationDistance)
                {
                    var bodySizeFactor = state.EnemyData.BodyScale * state.transform.localScale.x;
                    _separation += pushAway.normalized * ((minSeparationDistance - dist) * bodySizeFactor);
                }
                else
                {
                    _separation += pushAway.normalized * 0.05f;
                }
            }
            _cachedNearbyEnemies.Clear();
            
            if (InRange(state, _data.FleeDetectionRange)) return;
            
            state.SwitchState(_returnState.Invoke());
        }
    }
}