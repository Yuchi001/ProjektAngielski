using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Other;
using ParticlesPack.Enums;
using ParticlesPack.SO;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace ParticlesPack
{
    public class ParticleManager : MonoBehaviour
    {
        private readonly Dictionary<EParticlesType, ParticlePool> _particlesDict = new();
        
        private static ParticleManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            GameManager.OnGMStart += Init;
        }

        private void OnDisable()
        {
            GameManager.OnGMStart -= Init;
        }

        private void Init()
        {
            var soList = Resources.LoadAll<SoParticles>("ParticleSystems");
            
            foreach (var so in soList)
            {
                var manager = new GameObject(so.ParticlesType + "Manager");
                var script = manager.AddComponent<ParticlePool>();
                script.Init(so);
                _particlesDict.Add(so.ParticlesType, script);
            }
        }

        public static void SpawnParticles(EParticlesType type, Vector2 position)
        {
            if (!Instance._particlesDict.TryGetValue(type, out var manager)) return;

            manager.SpawnParticles(position);
        }

        public class ParticlePool : PoolManager
        {
            private ObjectPool<ParticlesPoolObject> _pool;
            private SoParticles _data;
            
            public void Init(SoParticles data)
            {
                _data = data;
                PoolSize = data.PoolSize;
                maxUpdateStackDuration = data.MaxUpdateTime;
                _pool = PoolHelper.CreatePool(this, data.ParticlesPrefab, true);
                
                PrepareQueue();
            }

            private void Update()
            {
                RunUpdatePoolStack();
            }

            protected override T GetPoolObject<T>()
            {
                return _pool.Get() as T;
            }

            public void SpawnParticles(Vector2 position)
            {
                GetPoolObject<PoolObject>().transform.position = position;
            }

            public override void ReleasePoolObject(PoolObject poolObject)
            {
                _pool.Release((ParticlesPoolObject)poolObject);
            }

            public override SoPoolObject GetRandomPoolData()
            {
                return _data;
            }
        }
    }
}