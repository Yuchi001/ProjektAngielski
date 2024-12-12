using System;
using Managers.Enums;
using PoolPack;
using UnityEngine;

namespace Managers.Base
{
    public abstract class SpawnerBase : PoolManager
    {
        [SerializeField] private float _waitBeforeSpawn = 1.5f;
        
        protected ESpawnerState _state = ESpawnerState.Stop;
        protected abstract float MaxTimer { get; }
        private float _timer = 0;

        private float _waitTimer = 0;
        
        protected virtual void Update()
        {
            if (_waitTimer < _waitBeforeSpawn)
            {
                _waitTimer += Time.deltaTime;
                return;
            }
            
            if (_state == ESpawnerState.Stop) return;
            
            
            _timer += Time.deltaTime;
            if (_timer < MaxTimer) return;
            _timer = 0;

            SpawnLogic();
        }

        public abstract void SpawnRandomEntity(Vector2 position);

        public void SetState(ESpawnerState state)
        {
            _state = state;
        }

        protected abstract void SpawnLogic();

        public T As<T>() where T : SpawnerBase
        {
            return (T)this;
        }
    }
}