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
        private readonly Action<EnemyLogic> _onPlayerInDetectionRange;
        private readonly Action<EnemyLogic> _onPlayerOutOfChaseRange = null;
        
        private Transform _transform;

        public ChaseState(ChaseStateData data, Action<EnemyLogic> onPlayerInDetectionRange, Action<EnemyLogic> onPlayerOutOfChaseChaseRange = null)
        {
            _onPlayerInDetectionRange = onPlayerInDetectionRange;
            _onPlayerOutOfChaseRange = onPlayerOutOfChaseChaseRange;
            _data = data;
        }

        public override void Enter(EnemyLogic state)
        {
            _transform = state.transform;
        }

        public override void Execute(EnemyLogic state)
        {
            var deltaTime = state.deltaTime;
             
            var position = _transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var separation = Vector2.zero;
            foreach (var poolObj in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == _transform.gameObject) continue;
                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            _transform.position += (Vector3)(finalDir * (deltaTime * _data.ChaseMovementSpeed));

            if (_onPlayerOutOfChaseRange != null && !InRange(state, _data.ChaseStopRange))
            {
                _onPlayerOutOfChaseRange.Invoke(state);
                return;
            }

            if (InRange(state, _data.ChaseDetectionRange)) _onPlayerInDetectionRange.Invoke(state);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }

        public class ChaseStateData : StateDataBase
        {
            [SerializeField] private float chaseMovementSpeed;
            [SerializeField] private float chaseDetectionRange;
            [SerializeField] private float chaseStopRange;

            public float ChaseMovementSpeed => chaseMovementSpeed;
            public float ChaseDetectionRange => chaseDetectionRange;
            public float ChaseStopRange => chaseStopRange;
        }
    }
}