using System;
using Managers;
using Managers.Other;
using PoolPack;
using UnityEngine.Pool;

namespace ProjectilePack
{
    public class ProjectileManager : PoolManager
    {
        private ObjectPool<Projectile> _pool;
        private static ProjectileManager Instance { get; set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            var prefab = GameManager.GetPrefab<Projectile>(PrefabNames.Projectile);
            _pool = PoolHelper.CreatePool(this, prefab, false);
            PrepareQueue();
        }

        protected override T GetPoolObject<T>()
        {
            return _pool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _pool.Release((Projectile)poolObject);
        }

        public static Projectile SpawnProjectile()
        {
            return Instance.GetPoolObject<Projectile>();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }
    }
}