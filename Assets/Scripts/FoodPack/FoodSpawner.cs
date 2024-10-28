using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Base;
using MarkerPackage;
using Other;
using Other.Enums;
using Other.SO;
using PlayerPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FoodPack
{
    public class FoodSpawner : SpawnerBase
    {
        [SerializeField] private float foodSpawnChance;
        [SerializeField] private float trySpawnRate;

        private readonly List<Food> _foodPool = new();

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

            PreparePool(_foodPool);
        }

        protected override void SpawnLogic()
        {
            if (PlayerManager == null) return;
            
            var randomPercentage = Random.Range(0, 101);
            if (randomPercentage > foodSpawnChance) return;

            var food = GetFromPool(_foodPool);
            if (food == null) return;
            
            var soFood = GetRandomFood();
            food.SetBusy();
                
            MarkerManager.Instance.GetMarkerFromPool(EEntityType.Positive)
                .Setup(food, soFood)
                .SetReady();
        }

        private SoFood GetRandomFood()
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