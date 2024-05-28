using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Other;
using Other.Enums;
using Other.SO;
using PlayerPack;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace FoodPack
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float foodSpawnChance;
        [SerializeField] private float trySpawnRate;
        [SerializeField] private GameObject foodPrefab;

        private float _timer = 0;
        private float _biggestWeight;
        private float _weightSum;
        
        private float _spawnRangeX;
        private float _spawnRangeY;

        private readonly List<(float weight, SoFood food)> _foodWeightList = new();
        
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
            
            var bottomLeftCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            var topRightCorner = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            var cameraWidthInUnits = Mathf.Abs(topRightCorner.x - bottomLeftCorner.x);
            var cameraHeightInUnits = Mathf.Abs(topRightCorner.y - bottomLeftCorner.y);

            _spawnRangeX = cameraWidthInUnits / 2 + 1;
            _spawnRangeY = cameraHeightInUnits / 2 + 1;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < 1 / trySpawnRate) return;

            _timer = 0;
            
            var randomPercentage = Random.Range(0, 101);
            if (randomPercentage > foodSpawnChance) return;

            var food = GetRandomFood();
            
            SpawnEntity.InstantiateSpawnEntity()
                .Setup(foodPrefab)
                .SetEntityType(EEntityType.Positive)
                .SetSpawnAction((foodObj) => foodObj.GetComponent<Food>().Setup(food))
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