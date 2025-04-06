using System;
using Managers;
using Managers.Other;
using PoolPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using UnityEngine.Pool;

namespace SpecialEffectPack
{
    public class SpecialEffectManager : PoolManager
    {
        #region Singleton

        private static SpecialEffectManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            var prefab = GameManager.GetPrefab<ExplosionAnimation>(PrefabNames.ExplosionBase);
            _explosionPool = PoolHelper.CreatePool(this, prefab, false);
            
            PrepareQueue();
        }

        #endregion
        
        private ObjectPool<ExplosionAnimation> _explosionPool;
        
        protected override T GetPoolObject<T>()
        {
            return _explosionPool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _explosionPool.Release(poolObject as ExplosionAnimation);
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        public static void SpawnExplosion(ESpecialEffectType specialEffectType, Vector2 position, float range, Color color = default)
        {
            var anim = Instance.GetPoolObject<ExplosionAnimation>();
            anim.Setup(specialEffectType, range, color);
            anim.transform.position = position;
        }
    }
}