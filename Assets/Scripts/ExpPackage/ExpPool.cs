using ExpPackage.Enums;
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

        private void Start()
        {
            var gemPrefab = GameManager.Instance.GetPrefab<ExpGem>(PrefabNames.ExpGem);
            _expGemPool = PoolHelper.CreatePool(this, gemPrefab, false);
            
            PrepareQueue();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        public static void SpawnExpGem(EExpGemType gemType, Vector2 position)
        {
            Instance.GetPoolObject<ExpGem>().Setup(gemType, position);
        }

        protected override T GetPoolObject<T>()
        {
            return _expGemPool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _expGemPool.Release((ExpGem)poolObject);
        }

        public override SoPoolObject GetRandomPoolData()
        {
            return null;
        }
    }
}