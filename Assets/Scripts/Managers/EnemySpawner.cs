using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using EnemyPack.SO;
using ExpPackage.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float enemiesHpScale = 1;
        [SerializeField] private int maxEnemiesCount = 300;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Camera mainCamera;
        [Space(10)]
        [SerializeField] private AnimationCurve enemySpawnRateCurve;
        [SerializeField] private float enemySpawnRate;
        [Space]
        [SerializeField] private AnimationCurve hordeSpawnRateCurve;
        [SerializeField] private float hordeSpawnRate;
        [SerializeField] private int hordeEnemiesCount;
        [Space(10)]
        [SerializeField, Tooltip("In seconds")] private int maximumDifficultyTimeCap = 3600;
        [SerializeField] private List<GemRarityTuple> gemRarityList;

        private float _enemyTimer = 0;
        private float _hordeTimer = 0;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();
        
        private float _difficultyTimer = 0;

        public float EnemiesHpScale => enemiesHpScale;
        public int DeadEnemies { get; set; } = 0;
        
        private void Start()
        {
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
        }

        private void Update()
        {
            _difficultyTimer += Time.deltaTime;
            _enemyTimer += Time.deltaTime;
            _hordeTimer += Time.deltaTime;
            
            if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemiesCount) return;

            TrySpawnEnemy();
            TrySpawnHorde();
        }

        private SoEnemy GetEnemy(bool isHorde)
        {
            var validEnemies = _allEnemies.Where(e => e.IsHorde == isHorde).ToList();

            var sum = gemRarityList.Sum(g => g.weight);
            var randomInt = Random.Range(0, sum + 1) * (1 - Mathf.Clamp(_difficultyTimer / maximumDifficultyTimeCap, 0, 1));
            var pickedGemType = EExpGemType.Common;
            gemRarityList.Sort((a, b) => a.weight - b.weight);
            foreach (var gemRarityTuple in gemRarityList)
            {
                if (randomInt <= gemRarityTuple.weight)
                {
                    pickedGemType = gemRarityTuple.gemType;
                    break;
                }

                randomInt -= gemRarityTuple.weight;
            }

            validEnemies = validEnemies.Where(e => e.ExpGemType == pickedGemType).ToList();
            
            return validEnemies.Count != 0 ? validEnemies[Random.Range(0, validEnemies.Count)] : _allEnemies[0];
        }

        private void TrySpawnEnemy()
        {
            var spawnRate = enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * enemySpawnRate;
            if (_enemyTimer < 1 / spawnRate || PlayerManager == null) return;

            _enemyTimer = 0;

            var enemySo = GetEnemy(false);
            
            SpawnEntity.InstantiateSpawnEntity()
                .Setup(enemyPrefab)
                .SetEntityType(EEntityType.Negative)
                .SetScale(enemySo.BodyScale)
                .SetSpawnAction((spawnedObj) => SetupEnemy(spawnedObj, enemySo))
                .SetReady();
        }

        private void TrySpawnHorde()
        {
            var spawnRate = hordeSpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * hordeSpawnRate;
            if (_hordeTimer < 1 / spawnRate || PlayerManager == null) return;

            _hordeTimer = 0;
            
            var hordeEnemy = GetEnemy(true);
            StartCoroutine(SpawnWave(hordeEnemy));
        }

        private IEnumerator SpawnWave(SoEnemy hordeEnemy)
        {
            for (var k = 0; k < 4; k++)
            {
                for (int i = 0, j = 0; i < hordeEnemiesCount / 4; i++, j += hordeEnemiesCount % i == 4 ? 1 : 0)
                {
                    SpawnEntity.InstantiateSpawnEntity()
                        .Setup(enemyPrefab)
                        .SetEntityType(EEntityType.Negative)
                        .SetScale(hordeEnemy.BodyScale)
                        .SetSpawnAction((spawnedObj) => SetupEnemy(spawnedObj, hordeEnemy))
                        .SetReady();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SetupEnemy(GameObject enemyObj, SoEnemy enemy)
        {
            var enemyScript = enemyObj.GetComponent<EnemyLogic>();
            var scale = enemy.BodyScale;
            enemyObj.transform.localScale = new Vector3(scale, scale, scale);
            
            enemyScript.Setup(enemy, PlayerManager.transform, this);
        }
    }

    [System.Serializable]
    public class GemRarityTuple
    {
        public EExpGemType gemType;
        public int weight;
    }
}