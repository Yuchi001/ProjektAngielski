using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using PlayerPack;
using UnityEngine;
using WorldGenerationPack;

namespace EnemyPack.States
{
    public class ChaseState : StateBase
    {
        private readonly ChaseStateData _data;
        private readonly Func<StateBase> _inRangeState;
        private readonly Func<StateBase> _outOfRangeState;

        private Vector2 _separation;
        private Transform _transform;
        private List<EnemyLogic> _cachedNearbyEnemies = new();

        public ChaseState(SoEnemy enemyData, Func<StateBase> inRangeState, Func<StateBase> outOfRangeStateBase = null) : base(enemyData)
        {
            _data = enemyData.GetStateData<ChaseStateData>();
            _inRangeState = inRangeState;
            _outOfRangeState = outOfRangeStateBase;
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _transform = state.transform;
        }

        public override void Execute(EnemyLogic state)
        {
            var position = _transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var finalDir = (direction + _separation * (0.1f * state.transform.localScale.x)).normalized;

            var ms = _data.ChaseMovementSpeed * SlowModificator(state) * Time.deltaTime;
            _transform.position += (Vector3)(finalDir * ms);

            if (_outOfRangeState != null && !InRange(state, _data.ChaseStopRange))
            {
                state.SwitchState(_outOfRangeState.Invoke());
                return;
            }

            if (InRange(state, _data.ChaseDetectionRange)) state.SwitchState(_inRangeState.Invoke());
        }

        public override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            _separation = Vector2.zero;
            const float minSeparationDistance = 1.5f;

            EnemyManager.GetNearbyEnemies(state.transform.position, minSeparationDistance, ref _cachedNearbyEnemies);
            foreach (var poolObj in _cachedNearbyEnemies)
            {
                if (poolObj.gameObject == _transform.gameObject) continue;

                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
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
        }
    }
}