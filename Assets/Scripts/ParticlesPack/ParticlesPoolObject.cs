using System;
using Other;
using ParticlesPack.SO;
using PoolPack;
using UnityEngine;

namespace ParticlesPack
{
    public class ParticlesPoolObject : PoolObject
    {
        private PoolManager _poolManager;
        
        private ParticleSystem _particleSystem;
        private float _lifeTime;

        private float _timer = 0;
        
        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);

            var data = (SoParticles)so;

            _particleSystem.Play();
            _lifeTime = data.LifeTime;
            _timer = 0;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _lifeTime) return;
            
            _poolManager.ReleasePoolObject(this);
        }

        public override void OnCreate(PoolManager poolManager)
        {
            _poolManager = poolManager;
            _particleSystem = GetComponent<ParticleSystem>();
            _particleSystem.Clear();
            _particleSystem.Stop();

            base.OnCreate(poolManager);
        }

        public override void OnRelease()
        {
            _particleSystem.Clear();
            _particleSystem.Stop();
            
            base.OnRelease();
        }
    }
}