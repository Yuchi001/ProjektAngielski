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
        private float _time;

        private const string PARTICLE_TIMER_ID = "PARTICLE_TIMER";
        

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);

            var data = (SoParticles)so;
            
            _particleSystem.Play();
            _time = data.LifeTime;
            SetTimer(PARTICLE_TIMER_ID);
        }

        public override void InvokeUpdate()
        {
            if (CheckTimer(PARTICLE_TIMER_ID) < _time) return;
            
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