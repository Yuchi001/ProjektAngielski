using System;
using Managers.Enums;
using UnityEngine;

namespace Managers.Base
{
    public abstract class SpawnerBase : MonoBehaviour
    {
        protected ESpawnerState _state = ESpawnerState.Stop;
        protected abstract float MaxTimer { get; }
        private float _timer = 0;
        
        protected virtual void Update()
        {
            if (_state == ESpawnerState.Stop) return;
            
            _timer += Time.deltaTime;
            if (_timer < MaxTimer) return;
            _timer = 0;

            SpawnLogic();
        }

        public void SetState(ESpawnerState state)
        {
            _state = state;
        }

        protected abstract void SpawnLogic();
    }
}