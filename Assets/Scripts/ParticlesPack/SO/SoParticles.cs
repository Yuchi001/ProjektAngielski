using Other;
using UnityEngine;

namespace ParticlesPack.SO
{
    [CreateAssetMenu(fileName = "new Particles", menuName = "Custom/Particles")]
    public class SoParticles : SoPoolObject
    {
        [SerializeField] private EParticlesType particlesType;
        [SerializeField] private ParticlesPoolObject particlesPrefab;
        [SerializeField] private int poolSize;
        [SerializeField] private float maxUpdateTime;

        public EParticlesType ParticlesType => particlesType;
        public ParticlesPoolObject ParticlesPrefab => particlesPrefab;
        public float LifeTime => particlesPrefab.GetComponent<ParticleSystem>().main.duration + 0.1f;
        public int PoolSize => poolSize;
        public float MaxUpdateTime => maxUpdateTime;
    }
}