using System;
using System.Collections;
using System.Threading.Tasks;
using Managers;
using Managers.Other;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace DamageIndicatorPack
{
    public class IndicatorManager : PoolManager
    {
        #region Singleton

        private static IndicatorManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion
        
        private ObjectPool<DamageIndicator> _pool;

        private void Start()
        {
            var prefab = GameManager.Instance.GetPrefab<DamageIndicator>(PrefabNames.DamageIndicatorHolder);
            _pool = PoolHelper.CreatePool(this, prefab, false);
            
            PrepareQueue();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        protected override T GetPoolObject<T>()
        {
            return _pool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _pool.Release((DamageIndicator)poolObject);
        }

        public static void SpawnIndicator(Vector2 position, int value, bool isCrit, bool isDamage = true)
        {
            Instance.GetPoolObject<DamageIndicator>().Setup(position, value, isDamage, isCrit);
        }
    }
}