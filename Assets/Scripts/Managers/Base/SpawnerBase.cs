using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.Enums;
using Other;
using UnityEngine;

namespace Managers.Base
{
    public abstract class SpawnerBase : MonoBehaviour
    {
        [SerializeField] protected GameObject spawnPrefab;
        [SerializeField] protected int poolDefaultSize;
        [SerializeField] private float _waitBeforeSpawn = 1.5f;
        
        protected ESpawnerState _state = ESpawnerState.Stop;
        protected abstract float MaxTimer { get; }
        private float _timer = 0;

        private float _waitTimer = 0;
        private bool _poolReady = false;
        
        protected void PreparePool<T>(List<T> pool) where T : EntityBase
        {
            spawnPrefab.SetActive(false);
            for (var i = 0; i < poolDefaultSize; i++)
            {
                var obj = Instantiate(spawnPrefab);
                var script = obj.GetComponent<T>();
                script.SpawnSetup(this);
                pool.Add(script);
            }

            _poolReady = true;
        }

        private void OnDisable()
        {
            spawnPrefab.SetActive(true);
        }

        protected virtual void Update()
        {
            if (_waitTimer < _waitBeforeSpawn)
            {
                _waitTimer += Time.deltaTime;
                return;
            }
            
            if (_state == ESpawnerState.Stop || !_poolReady) return;
            
            
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
        
        protected EntityBase GetFromPool<T>(List<T> pool) where T : EntityBase
        {
            var enemy = pool.FirstOrDefault(f => !f.gameObject.activeInHierarchy && !f.Active);
            if (enemy != default) return enemy;
            
            PreparePool(pool);
            return pool.FirstOrDefault(f => !f.gameObject.activeInHierarchy && !f.Active);
        }

        public T As<T>() where T : SpawnerBase
        {
            return (T)this;
        }
    }
}