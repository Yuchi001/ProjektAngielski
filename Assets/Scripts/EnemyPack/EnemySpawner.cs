using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using ExpPackage.Enums;
using Managers;
using Managers.Base;
using Managers.Enums;
using MarkerPackage;
using Other;
using Other.Enums;
using PlayerPack;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    [RequireComponent(typeof(EnemyManager))]
    public class EnemySpawner : SpawnerBase
    {
        [SerializeField] private float enemiesHpScaleMultiplierPerKill = 0.001f;
        [SerializeField] private float enemiesHpScale = 1;
        [Space(10)]
        [SerializeField] private AnimationCurve enemySpawnRateCurve;
        [SerializeField] private float enemySpawnRateMultiplierPerKill = 0.001f;
        [SerializeField] private float enemySpawnRate;
        [Space(10)]
        [SerializeField, Tooltip("In seconds")] private int maximumDifficultyTimeCap = 3600;
        [SerializeField] private List<GemRarityTuple> gemRarityList;
        private static PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();

        public int ShootingEnemiesCount { get; private set; } = 0;
        
        private float _difficultyTimer = 0;

        public delegate void EnemyDieDelegate(EnemyLogic enemyLogic);
        public static event EnemyDieDelegate OnEnemyDie;

        private readonly List<EnemyLogic> _enemyPool = new();
 
        public int DeadEnemies { get; private set; }

        private float EnemySpawnRate => enemySpawnRate + 1f * enemySpawnRateMultiplierPerKill * DeadEnemies;
        protected override float MaxTimer => 1f / (enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * EnemySpawnRate);
        public float EnemiesHpScale => enemiesHpScale + Mathf.Pow(1f + DeadEnemies * enemiesHpScaleMultiplierPerKill, 2) - 1;
        

        private void Awake()
        {
            PreparePool(_enemyPool);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.MapGenerator != null);
            
            GetComponent<EnemyManager>().Setup(_enemyPool);
            
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
        }

        protected override void Update()
        {
            base.Update();

            if (_state == ESpawnerState.Stop) return;
            _difficultyTimer += Time.deltaTime;
        }
        
        public void IncrementDeadEnemies(EnemyLogic enemyLogic, SoEnemy enemy)
        {
            OnEnemyDie?.Invoke(enemyLogic);
            DeadEnemies++;
            if (enemy.CanShoot) ShootingEnemiesCount--;
        }

        protected override void SpawnLogic()
        {
            if (PlayerManager == null) return;
            
            var enemy = GetFromPool(_enemyPool);
            if (enemy == null) return;
            
            var enemySo = GetEnemy(false);
            enemy.SetBusy();
            
            if (enemySo.CanShoot) ShootingEnemiesCount++;
                
            MarkerManager.Instance.GetMarkerFromPool(EEntityType.Negative)
                .Setup(enemy, enemySo)
                .SetScale(enemySo.BodyScale)
                .SetReady();
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

        public void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            var enemyObj = GetFromPool(_enemyPool);
            if (enemyObj == null) return;
            
            var enemyScript = enemyObj.As<EnemyLogic>();
            enemyObj.transform.position = position;
            
            enemyScript.Setup(enemy);
            if (enemy.CanShoot) ShootingEnemiesCount++;
        }

        public List<EnemyLogic> GetActiveEnemies()
        {
            return _enemyPool.Where(e => e.Active && e.gameObject.activeInHierarchy).ToList();
        }
    }

    [System.Serializable]
    public class GemRarityTuple
    {
        public EExpGemType gemType;
        public int weight;
    }
}