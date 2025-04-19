using System;
using System.Collections.Generic;
using System.Linq;
using DifficultyPack;
using EnemyPack.SO;
using GameLoaderPack;
using Managers;
using Managers.Base;
using Managers.Other;
using MapPack;
using MarkerPackage;
using Other;
using Other.Enums;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;
using Utils;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    // TODO: create virtual methods
    public abstract class EnemySpawner : SpawnerBase, IMissionDependentInstance
    {
        [SerializeField] private float maxDistanceFromPlayer = 20;
        [SerializeField] private float standardDifficultyDeviation = 0.2f;
        
        private readonly Queue<SoEnemy> _despawnQueue = new();

        private List<SoEnemy> _allEnemies = new();

        public delegate void EnemyDieDelegate(EnemyLogic enemyLogic);
        public static event EnemyDieDelegate OnEnemyDie;

        private ObjectPool<EnemyLogic> _enemyPool;
        
        public int DeadEnemies { get; private set; }
        
        private MapManager.MissionData _currentMission;
        private float _timer = 0;

        private float ScaledDifficulty => _currentMission.GetScaledDifficulty(_timer);
        protected override float MaxTimer => 1f / DifficultyManager.GetEnemySpawnRate(ScaledDifficulty);

        private void Awake()
        {
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
            
            var enemyPrefab = GameManager.GetPrefab<EnemyLogic>(PrefabNames.Enemy);
            _enemyPool = PoolHelper.CreatePool(this, enemyPrefab, true);
            PrepareQueue();
        }

        public override void Init(MapManager.MissionData currentMission)
        {
            _allEnemies = _allEnemies.Where(e => e.OccurenceList.Contains(currentMission.RegionType)).ToList();
            _currentMission = currentMission;
        }

        protected override void Update()
        {
            _timer += Time.deltaTime;
            RunUpdatePoolStack();
            
            base.Update();
        }

        protected override PoolObject InvokeQueueUpdate()
        {
            var current = base.InvokeQueueUpdate();
            if (!current.transform.InRange(PlayerManager.PlayerPos, maxDistanceFromPlayer))
            {
                _despawnQueue.Enqueue(current.As<EnemyLogic>().EnemyData);
                ReleasePoolObject(current);
            }

            return current;
        }

        public void IncrementDeadEnemies(EnemyLogic enemyLogic, SoEnemy enemy)
        {
            OnEnemyDie?.Invoke(enemyLogic);
            DeadEnemies++;
        }

        protected override void SpawnLogic()
        {
            if (!PlayerManager.HasInstance() || _enemyPool == null) return;

            MarkerManager.SpawnMarker(this, EEntityType.Negative);
        }

        public void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            var enemyObj = _enemyPool.Get();
            enemyObj.OnGet(enemy);
            enemyObj.transform.position = position;
        }

        public override void SpawnRandomEntity(Vector2 position)
        {
            SpawnEnemy(GetRandomPoolData() as SoEnemy, position);
        }

        public List<PoolObject> GetActiveEnemies()
        {
            return ActiveObjects;
        }

        protected override T GetPoolObject<T>()
        {
            return _enemyPool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _enemyPool.Release(poolObject as EnemyLogic);
        }

        public override SoPoolObject GetRandomPoolData()
        {
            if (_despawnQueue.Any()) return _despawnQueue.Dequeue();

            var enemies = GetValidEnemies();
            var soEnemies = enemies as SoEnemy[] ?? enemies.ToArray();
            return soEnemies.ElementAt(Random.Range(0, soEnemies.Length));

            IEnumerable<SoEnemy> GetValidEnemies()
            {
                var current = new List<SoEnemy>();
                var pickedDifficulty = GetRandomDifficulty();
                foreach (var enemy in _allEnemies) if (enemy.Difficulty <= pickedDifficulty) current.Add(enemy);
                return current.Count > 0 ? current : new List<SoEnemy> {_allEnemies[0]};
            }
        }
        
        private int GetRandomDifficulty()
        {
            var bounds = _currentMission.Difficulty.EnemyDifficultyBounds();
            var maxDifficulty = Mathf.CeilToInt(Mathf.Lerp(bounds.Min, bounds.Max, ScaledDifficulty));
            
            var mean = Mathf.Lerp(1f, maxDifficulty, ScaledDifficulty);
            var stdDev = maxDifficulty * standardDifficultyDeviation;

            var weights = new List<float>();
            for (var i = 1; i <= maxDifficulty; i++)
            {
                var weight = Mathf.Exp(-Mathf.Pow(i - mean, 2) / (2 * stdDev * stdDev));
                weights.Add(weight);
            }

            var totalWeight = weights.Sum();
            var rand = Random.value * totalWeight;
            var cumulative = 0f;

            for (var i = 0; i < weights.Count; i++)
            {
                cumulative += weights[i];
                if (rand <= cumulative) return i + 1;
            }

            throw new Exception("There should always be a difficulty to find");
        }
    }
}