using System;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace InventoryPack.WorldItemPack
{
    public class ThrowState : IWorldItemState
    {
        private readonly Func<LiveState> _nextState;
        private readonly MinMax _throwSpeed;
        private readonly float _slowMagnitude;

        private Vector2 _randomDir;
        private float _currentThrowSpeed;

        public ThrowState(MinMax throwSpeed, float slowMagnitude, Func<LiveState> nextState)
        {
            _slowMagnitude = slowMagnitude;
            _throwSpeed = throwSpeed;
            _nextState = nextState;
        }
        
        public void Enter(WorldItem item)
        {
            _randomDir = new Vector2
            {
                x = Random.Range(-1f, 1f),
                y = Random.Range(-1f, 1f),
            }.normalized;
            _currentThrowSpeed = _throwSpeed.RandomFloat();
        }

        public void Execute(WorldItem item)
        {
            _currentThrowSpeed -= _slowMagnitude * Time.deltaTime;
            item.transform.Translate(_randomDir * (_currentThrowSpeed * Time.deltaTime));

            if (_currentThrowSpeed > 0) return;
            
            item.SwitchState(_nextState.Invoke());
        }

        public void FixedExecute(WorldItem item)
        {
            
        }
    }
}