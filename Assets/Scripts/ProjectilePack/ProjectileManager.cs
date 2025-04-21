using System;
using ItemPack;
using ItemPack.SO;
using Managers;
using Managers.Other;
using PoolPack;
using UnityEngine.Pool;

namespace ProjectilePack
{
    public class ProjectileManager : PoolManager
    {
        public static string PLAYER_TAG = "Player";
        public static string ENEMY_TAG = "Enemy";
        
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

        public static Projectile SpawnProjectile(IProjectileMovementStrategy movementStrategy, int damage, string targetTag)
        {
            return Instance.GetPoolObject<Projectile>().Setup(movementStrategy, damage, targetTag);
        }
        
        public static Projectile SpawnProjectile(IProjectileMovementStrategy movementStrategy, ItemLogicBase item)
        {
            return Instance.GetPoolObject<Projectile>().Setup(movementStrategy, item.Damage, ENEMY_TAG, item.InventoryItem.ItemTags);
        }

        public static void ReturnProjectile(Projectile projectile)
        {
            Instance.ReleasePoolObject(projectile);
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }
    }
}