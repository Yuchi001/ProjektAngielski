using System;
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
        private readonly StateBase _inRangeState;
        private readonly StateBase _outOfRangeState;
        
        private Transform _transform;

        public ChaseState(SoEnemy enemyData, StateBase inRangeState, StateBase outOfRangeStateBase = null) : base(enemyData)
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
            var deltaTime = state.deltaTime;
            var position = _transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var separation = Vector2.zero;
            var minSeparationDistance = 1.5f;

            foreach (var poolObj in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == _transform.gameObject) continue;

                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
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

            var finalDir = (direction + separation * (0.1f * state.transform.localScale.x)).normalized;

            _transform.position += (Vector3)(finalDir * (deltaTime * _data.ChaseMovementSpeed));

            if (_outOfRangeState != null && !InRange(state, _data.ChaseStopRange))
            {
                state.SwitchState(_outOfRangeState);
                return;
            }

            if (InRange(state, _data.ChaseDetectionRange)) state.SwitchState(_inRangeState);
        }


        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}