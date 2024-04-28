using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using EnemyPack.SO;
using ExpPackage.Enums;
using PlayerPack;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EnemySpawner : MonoBehaviour
    {
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
        private float _spawnRangeX;
        private float _spawnRangeY;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();

        private float _difficultyTimer = 0;
        
        private void Start()
        {
            var bottomLeftCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            var topRightCorner = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            var cameraWidthInUnits = Mathf.Abs(topRightCorner.x - bottomLeftCorner.x);
            var cameraHeightInUnits = Mathf.Abs(topRightCorner.y - bottomLeftCorner.y);

            _spawnRangeX = cameraWidthInUnits / 2 + 1;
            _spawnRangeY = cameraHeightInUnits / 2 + 1;

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

            return validEnemies[Random.Range(0, validEnemies.Count)];
        }

        private void TrySpawnEnemy()
        {
            var spawnRate = enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * enemySpawnRate;
            if (_enemyTimer < 1 / spawnRate || PlayerManager == null) return;

            _enemyTimer = 0;
            
            // true => x
            // false => y
            var randomDimension = UtilsMethods.RandomClamp(true, false);
            
            var xDiff = randomDimension ? 
                Random.Range(-_spawnRangeX, _spawnRangeX) : 
                UtilsMethods.RandomClamp(-_spawnRangeX, _spawnRangeX);
            
            var yDiff = !randomDimension ? 
                Random.Range(-_spawnRangeY, _spawnRangeY) : 
                UtilsMethods.RandomClamp(-_spawnRangeY, _spawnRangeY);
            
            var spawnPos = PlayerManager.transform.position;
            spawnPos.x += xDiff;
            spawnPos.y += yDiff;
            
            InstantiateEnemy(GetEnemy(false), spawnPos);
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
                    var xDiff = j % 2 == 0 ? i % 2 == 0 ? -_spawnRangeX : _spawnRangeX : Random.Range(-_spawnRangeX, _spawnRangeX);
                    var yDiff = j % 2 == 0 ? Random.Range(-_spawnRangeY, _spawnRangeY) : i % 2 == 0 ? -_spawnRangeY : _spawnRangeY;
                    
                    var spawnPos = PlayerManager.transform.position;
                    spawnPos.x += xDiff;
                    spawnPos.y += yDiff;

                    InstantiateEnemy(hordeEnemy, spawnPos);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void InstantiateEnemy(SoEnemy enemy, Vector2 spawnPos)
        {
            var enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            var enemyScript = enemyObj.GetComponent<EnemyLogic>();
            var scale = enemy.BodyScale;
            enemyObj.transform.localScale = new Vector3(scale, scale, scale);
            
            enemyScript.Setup(enemy, PlayerManager.transform);
        }
    }

    [System.Serializable]
    public class GemRarityTuple
    {
        public EExpGemType gemType;
        public int weight;
    }
}