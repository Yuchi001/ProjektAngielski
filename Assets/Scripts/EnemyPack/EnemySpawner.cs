using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.Enums;
using EnemyPack.SO;
using ExpPackage.Enums;
using Managers;
using Managers.Base;
using Managers.Enums;
using Managers.Other;
using MarkerPackage;
using Other;
using Other.Enums;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;
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

        private ObjectPool<EnemyLogic> _enemyPool;
 
        public int DeadEnemies { get; private set; }

        private float EnemySpawnRate => enemySpawnRate + 1f * enemySpawnRateMultiplierPerKill * DeadEnemies;
        protected override float MaxTimer => 1f / (enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * EnemySpawnRate);
        public float EnemiesHpScale => enemiesHpScale + Mathf.Pow(1f + DeadEnemies * enemiesHpScaleMultiplierPerKill, 2) - 1;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.MapGenerator != null);
            
            GetComponent<EnemyManager>().Setup(ActiveObjects);
            
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
            
            var enemyPrefab = GameManager.Instance.GetPrefab(PrefabNames.Enemy);
            _enemyPool = PoolHelper.CreatePool<EnemyLogic>(this, enemyPrefab, poolDefaultSize);
            PrepareQueue();
        }

        protected override void Update()
        {
            RunUpdatePoolStack();
            
            base.Update();

            if (_state == ESpawnerState.Stop) return;
            _difficultyTimer += Time.deltaTime;
        }
        
        public void IncrementDeadEnemies(EnemyLogic enemyLogic, SoEnemy enemy)
        {
            OnEnemyDie?.Invoke(enemyLogic);
            DeadEnemies++;
            if (enemy.ShootType != EShootType.None) ShootingEnemiesCount--;
        }

        protected override void SpawnLogic()
        {
            if (PlayerManager == null || _enemyPool == null) return;

            _enemyPool.Get();
            
            //TODO: tutaj jakos zastapic
            //if (enemySo.ShootType != EShootType.None) ShootingEnemiesCount++;
                
            MarkerManager.Instance.GetMarkerFromPool(EEntityType.Negative)
                .Setup(this)
                .SetReady();
        }

        public void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            var enemyObj = _enemyPool.Get();
            enemyObj.OnGet(enemy);
            enemyObj.transform.position = position;
            
            // TODO: Tutaj tez zastap
            //if (enemy.ShootType != EShootType.None) ShootingEnemiesCount++;
        }

        public List<PoolObject> GetActiveEnemies()
        {
            return ActiveObjects;
        }

        public override PoolObject GetPoolObject()
        {
            return _enemyPool.Get();
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _enemyPool.Release(poolObject as EnemyLogic);
        }

        public override SoEntityBase GetRandomPoolData()
        {
            var isHorde = false; // TODO: do usuniecia
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
    }

    [System.Serializable]
    public class GemRarityTuple
    {
        public EExpGemType gemType;
        public int weight;
    }
}