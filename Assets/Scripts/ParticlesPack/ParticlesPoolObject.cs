using Other;
using ParticlesPack.SO;
using PoolPack;
using UnityEngine;

namespace ParticlesPack
{
    public class ParticlesPoolObject : PoolObject
    {
        private ParticleManager.ParticlePool _particleManager;
        
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

        public override void InvokeUpdate()
        {
            base.InvokeUpdate();
            _timer += deltaTime;
            if (_timer < _lifeTime) return;
            
            _particleManager.ReleasePoolObject(this);
        }

        public override void OnCreate(PoolManager poolManager)
        {
            _particleManager = (ParticleManager.ParticlePool)poolManager;
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