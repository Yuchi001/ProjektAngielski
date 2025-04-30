using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;
using Utils;

namespace EnemyPack.States
{
    public class PatrolState : StateBase
    {
        private Vector2 _startPos;
        private Vector2 _currentDestination;

        private readonly PatrolStateData _data;
        private readonly Func<StateBase> _newDestinationState;

        public PatrolState(SoEnemy data, Func<StateBase> newDestinationState = null) : base(data)
        {
            _newDestinationState = newDestinationState;
            _data = data.GetStateData<PatrolStateData>();
        }
        
        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _startPos = state.transform.position;
            _currentDestination = state.transform.RandomPointInRange(_data.PatrolRange);
        }

        public override void Execute(EnemyLogic state)
        {
            var enemyTransform = state.transform;
            enemyTransform.MoveTowards(_currentDestination, state.deltaTime * _data.PatrolMovementSpeed);
            
            if (!enemyTransform.InRange(_currentDestination, 0.15f)) return;

            _currentDestination = enemyTransform.RandomPointInRange(_startPos, _data.PatrolRange);
            if (_newDestinationState != null) state.SwitchState(_newDestinationState.Invoke());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }

        
    }
}