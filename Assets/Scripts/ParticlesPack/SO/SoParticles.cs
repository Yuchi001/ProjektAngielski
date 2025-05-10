using Other;
using ParticlesPack.Enums;
using UnityEngine;

namespace ParticlesPack.SO
{
    [CreateAssetMenu(fileName = "new Particles", menuName = "Custom/Particles")]
    public class SoParticles : SoPoolObject
    {
        [SerializeField] private EParticlesType particlesType;
        [SerializeField] private ParticlesPoolObject particlesPrefab;
        [SerializeField] private int poolSize;

        public EParticlesType ParticlesType => particlesType;
        public ParticlesPoolObject ParticlesPrefab => particlesPrefab;
        public float LifeTime => particlesPrefab.GetComponent<ParticleSystem>().main.duration + 0.1f;
        public int PoolSize => poolSize;
    }
}