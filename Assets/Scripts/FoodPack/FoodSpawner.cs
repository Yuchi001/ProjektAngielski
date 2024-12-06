using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Base;
using Managers.Other;
using MarkerPackage;
using Other;
using Other.Enums;
using Other.SO;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace FoodPack
{
    public class FoodSpawner : SpawnerBase
    {
        [SerializeField] private float foodSpawnChance;
        [SerializeField] private float trySpawnRate;

        private ObjectPool<Food> _foodPool;

        private float _biggestWeight;
        private float _weightSum;

        private readonly List<(float weight, SoFood food)> _foodWeightList = new();

        protected override float MaxTimer => trySpawnRate;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private void Awake()
        {
            var foodList = Resources.LoadAll<SoFood>("Food").Select(Instantiate).ToList();
            foodList.Sort((a, b) => a.SaturationValue - b.SaturationValue);
            _biggestWeight = foodList[^1].SaturationValue + 1;
            foreach (var food in foodList)
            {
                var weight = _biggestWeight - food.SaturationValue;
                _foodWeightList.Add((weight, food));
                _weightSum += weight;
            }
            _foodWeightList.Sort((a, b) => (int)a.weight - (int)b.weight);
        }

        private void Start()
        {
            var foodPrefab = GameManager.Instance.GetPrefab(PrefabNames.Food);
            _foodPool = PoolHelper.CreatePool<Food>(this, foodPrefab, 10);
        }

        protected override void SpawnLogic()
        {
            if (PlayerManager == null) return;
            
            var randomPercentage = Random.Range(0, 101);
            if (randomPercentage > foodSpawnChance) return;
                
            MarkerManager.Instance.GetMarkerFromPool(EEntityType.Positive)
                .Setup(this)
                .SetReady();
        }
        
        public override PoolObject GetPoolObject()
        {
            return _foodPool.Get();
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _foodPool.Release(poolObject as Food);
        }

        public override SoEntityBase GetRandomPoolData()
        {
            var randomNumber = Random.Range(0, _weightSum);
            
            foreach (var food in _foodWeightList)
            {
                if (randomNumber <= food.weight) return food.food;
                
                randomNumber -= food.weight;
            }

            return null;
        }
    }
}