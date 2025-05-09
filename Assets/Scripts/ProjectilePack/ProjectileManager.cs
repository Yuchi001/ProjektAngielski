﻿using System;
using ItemPack;
using ItemPack.SO;
using Managers;
using Managers.Other;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectilePack
{
    public class ProjectileManager : PoolManager
    {
        public const string PLAYER_TAG = "Player";
        public const string ENEMY_TAG = "Enemy";

        private ObjectPool<Projectile> _pool;
        private static ProjectileManager Instance { get; set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            var prefab = GameManager.GetPrefab<Projectile>(PrefabNames.Projectile);
            _pool = PoolHelper.CreatePool(this, prefab, false);
            PrepareQueue();
            
            GameManager.EnqueueUnloadGameAction(() => ClearAll(_pool));
        }

        protected override T GetPoolObject<T>()
        {
            return _pool.Get() as T;
        }

        public static int CountActive => Instance._pool.CountActive;

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            if (!poolObject.Active) return;
            _pool.Release((Projectile)poolObject);
        }

        public static Projectile SpawnProjectile(IProjectileMovementStrategy movementStrategy, int damage, Vector2 spawnPosition, string targetTag)
        {
            var projectile = Instance.GetPoolObject<Projectile>().Setup(movementStrategy, damage, targetTag);
            projectile.transform.position = spawnPosition;
            return projectile;
        }
        
        public static Projectile SpawnProjectile(IProjectileMovementStrategy movementStrategy, ItemLogicBase item)
        {
            var projectile = Instance.GetPoolObject<Projectile>().Setup(movementStrategy, item.Damage, ENEMY_TAG, item.InventoryItem.ItemTags);
            projectile.transform.position = PlayerManager.PlayerPos;
            return projectile;
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