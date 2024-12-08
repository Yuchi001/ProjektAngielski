using System;
using System.Collections;
using Managers;
using Managers.Other;
using Other;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace ExpPackage
{
    public class ExpPool : PoolManager
    {
        private ObjectPool<ExpGem> _expGemPool;

        #region Singleton

        public static ExpPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);
            
            var gemPrefab = GameManager.Instance.GetPrefab(PrefabNames.ExpGem);
            _expGemPool = PoolHelper.CreatePool<ExpGem>(this, gemPrefab);
            
            PrepareQueue();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        public override PoolObject GetPoolObject()
        {
            return _expGemPool.Get();
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _expGemPool.Release((ExpGem)poolObject);
        }

        public override SoEntityBase GetRandomPoolData()
        {
            return null;
        }
    }
}