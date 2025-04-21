using System;
using UnityEngine;
using Utils;

namespace EnemyPack.States
{
    public class Patrol : StateBase
    {
        private readonly float _patrolRange;
        private float _detectionRange;

        private Vector2 _startPos;
        private Vector2 _currentDestination;
        private Action<EnemyLogic> _onNewDestination = null;
        private Action<EnemyLogic> _onDetect = null;

        public Patrol(float patrolRange)
        {
            _patrolRange = patrolRange;
        }

        public Patrol(float patrolRange, Action<EnemyLogic> onNewDestination)
        {
            _patrolRange = patrolRange;
            _onNewDestination = onNewDestination;
        }

        public Patrol SetOnPlayerInRange(float detectionRange, Action<EnemyLogic> onDetect)
        {
            _detectionRange = detectionRange;
            _onDetect = onDetect;
            return this;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _startPos = state.transform.position;
            _currentDestination = state.transform.RandomPointInRange(_patrolRange);
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            var enemyTransform = state.transform;
            enemyTransform.MoveTowards(_currentDestination, deltaTime * state.EnemyData.MovementSpeed);
            
            if (_onDetect != null && enemyTransform.InRange(PlayerPos, _detectionRange)) _onDetect.Invoke(state);
            
            if (!enemyTransform.InRange(_currentDestination, 0.15f)) return;

            _currentDestination = enemyTransform.RandomPointInRange(_startPos, _patrolRange);
            _onNewDestination?.Invoke(state);
        }

        public override void Reset(EnemyLogic state)
        {
            _onNewDestination = null;
        }
    }
}