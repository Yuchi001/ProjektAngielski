using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Other;
using ParticlesPack.SO;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace ParticlesPack
{
    public class ParticleManager : MonoBehaviour
    {
        #region Singleton

        public static ParticleManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private readonly Dictionary<EParticlesType, ParticlePool> _particlesDict = new();
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);

            var soList = Resources.LoadAll<SoParticles>("ParticleSystems");
            
            foreach (var so in soList)
            {
                var manager = new GameObject(so.ParticlesType + "Manager");
                var script = manager.AddComponent<ParticlePool>();
                script.Setup(so);
                _particlesDict.Add(so.ParticlesType, script);
                yield return new WaitForEndOfFrame();
            }
        }

        public void SpawnParticles(EParticlesType type, Vector2 position)
        {
            if (!_particlesDict.TryGetValue(type, out var manager)) return;

            manager.GetPoolObject<PoolObject>().transform.position = position;
        }

        public class ParticlePool : PoolManager
        {
            private ObjectPool<ParticlesPoolObject> _pool;
            private SoParticles _data;
            
            public void Setup(SoParticles data)
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

            public override T GetPoolObject<T>()
            {
                return _pool.Get() as T;
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