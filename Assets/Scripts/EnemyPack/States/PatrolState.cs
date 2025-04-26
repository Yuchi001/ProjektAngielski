using System;
using EnemyPack.States.StateData;
using UnityEngine;
using Utils;

namespace EnemyPack.States
{
    public class PatrolState : StateBase
    {
        private float _detectionRange;

        private Vector2 _startPos;
        private Vector2 _currentDestination;
        private Action<EnemyLogic> _onNewDestination = null;
        private Action<EnemyLogic> _onDetect = null;

        private PatrolStateData _data;

        public PatrolState(PatrolStateData data, Action<EnemyLogic> onNewDestination = null)
        {
            _data = data;
            _onNewDestination = onNewDestination;
        }

        public PatrolState SetOnPlayerInRange(float detectionRange, Action<EnemyLogic> onDetect)
        {
            _detectionRange = detectionRange;
            _onDetect = onDetect;
            return this;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _startPos = state.transform.position;
            _currentDestination = state.transform.RandomPointInRange(_data.PatrolRange);
        }

        public override void Execute(EnemyLogic state)
        {
            var enemyTransform = state.transform;
            enemyTransform.MoveTowards(_currentDestination, state.deltaTime * _data.PatrolMovementSpeed);
            
            if (_onDetect != null && enemyTransform.InRange(PlayerPos, _detectionRange)) _onDetect.Invoke(state);
            
            if (!enemyTransform.InRange(_currentDestination, 0.15f)) return;

            _currentDestination = enemyTransform.RandomPointInRange(_startPos, _data.PatrolRange);
            _onNewDestination?.Invoke(state);
        }

        public override void Reset(EnemyLogic state)
        {
            _onNewDestination = null;
        }

        public class PatrolStateData : StateDataBase
        {
            [SerializeField] private float patrolMovementSpeed;
            [SerializeField] private float patrolRange;

            public float PatrolMovementSpeed => patrolMovementSpeed;
            public float PatrolRange => patrolRange;
        }
    }
}