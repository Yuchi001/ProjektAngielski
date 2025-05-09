﻿using System;
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
        private ObjectPool<WorldItem> _pool;
        private List<SoCoinItem> _coins;
        private SoHealingOrbItem _healingOrb;
        private SoSoulItem _soulItem;
        private SoScrapItem _scrapItem;

        #region Singleton

        private static WorldItemManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            GameManager.OnGMStart += GmStart;
        }

        #endregion

        private void OnDisable()
        {
            GameManager.OnGMStart -= GmStart;
        }

        private void GmStart()
        {
            _soulItem = Resources.Load<SoSoulItem>("Items/Soul");
            _scrapItem = Resources.Load<SoScrapItem>("Items/Scrap");
            _healingOrb = Resources.Load<SoHealingOrbItem>("Items/HealingOrb");
            _coins = Resources.LoadAll<SoCoinItem>("Items").ToList();
            _coins.Sort((c1, c2) => c2.ItemPrice - c1.ItemPrice);
            
            var prefab = GameManager.GetPrefab<WorldItem>(PrefabNames.WorldItem);
            _pool = PoolHelper.CreatePool(this, prefab, false);
            
            PrepareQueue();
        }

        private static void SpawnItem(SoItem data, Vector2 position, params int[] paramsArray)
        {
            var item = Instance.GetPoolObject<WorldItem>();
            item.Setup(data, position, paramsArray);
        }

        public static void SpawnInventoryItem(SoInventoryItem item, Vector2 position, int level)
        {
            SpawnItem(item, position, level);
        }

        public static void SpawnSouls(Vector2 position, int count)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnItem(Instance._soulItem, position);
            }
        }
        
        public static void SpawnScraps(Vector2 position, int count)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnItem(Instance._scrapItem, position);
            }
        }

        public static void ClearItems()
        {
            Instance.ClearAll(Instance._pool);
        }
        
        public static void SpawnHealingOrb(Vector2 position)
        {
            SpawnItem(Instance._healingOrb, position);
        }
        
        public static void SpawnCoins(int value, Vector2 position)
        {
            var coins = Instance._coins;
            foreach (var coin in coins)
            {
                var count = value / coin.ItemPrice;
                while (count > 0)
                {
                    SpawnItem(coin, position, coin.ItemPrice);
                    count--;
                }

                value -= (value / coin.ItemPrice) * coin.ItemPrice;

                if (value <= 0) return;
            }
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
            _pool.Release((WorldItem)poolObject);
        }
    }
}