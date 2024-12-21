using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemPack.SO;
using Managers;
using Managers.Other;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace InventoryPack.WorldItemPack
{
    public class WorldItemManager : PoolManager
    {
        [SerializeField] private List<CoinData> coinDataList;
        
        private ObjectPool<WorldItem> _pool;

        #region Singleton

        private static WorldItemManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);
            
            coinDataList.Sort((c1, c2) => c2.CoinValue - c1.CoinValue);

            var prefab = GameManager.Instance.GetPrefab<WorldItem>(PrefabNames.WorldItem);
            _pool = PoolHelper.CreatePool(this, prefab, false);
            
            PrepareQueue();
        }

        public static void SpawnItem(SoItem data, int level, Vector2 position)
        {
            var item = Instance.GetPoolObject<WorldItem>();
            item.Setup(data, level, position);
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        public static void SpawnCoins(int value, Vector2 position)
        {
            var coins = Instance.coinDataList;
            foreach (var coin in coins)
            {
                var count = value % coin.CoinValue;
                while (count > 0 && value >= coin.CoinValue)
                {
                    var item = Instance.GetPoolObject<WorldItem>();
                    item.Setup(coin.CoinSprite, coin.CoinValue, position);
                    count--;
                }
            }
        }
        
        protected override T GetPoolObject<T>()
        {
            return _pool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _pool.Release((WorldItem)poolObject);
        }

        [System.Serializable]
        public struct CoinData
        {
            [SerializeField] private Sprite coinSprite;
            [SerializeField] private int coinValue;

            public Sprite CoinSprite => coinSprite;
            public int CoinValue => coinValue;
        }
    }
}