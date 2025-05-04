using System;
using System.Collections.Generic;
using System.Linq;
using DifficultyPack;
using EnemyPack.SO;
using Managers;
using Managers.Other;
using MapPack;
using MarkerPackage;
using Other;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;
using Utils;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    public abstract class EnemySpawner : PoolManager, IUseMarker
    {
        [SerializeField] private float maxDistanceFromPlayer = 60;
        [SerializeField] private float standardDifficultyDeviation = 0.2f;
        [SerializeField] private float _waitBeforeSpawn = 1.5f;
        
        private readonly Queue<SoEnemy> _despawnQueue = new();
        private int _despawnCount = 0;

        private List<SoEnemy> _allEnemies = new();

        private ObjectPool<EnemyLogic> _enemyPool;
        
        private MapManager.MissionData _currentMission;
        
        private float _progression = 0;
        private float _timer = 0;
        private float _waitTimer = 0;
        
        protected bool _spawn = false;

        private float ScaledDifficulty => _currentMission.GetScaledDifficulty(_progression);
        private float MaxTimer => 1f / DifficultyManager.GetEnemySpawnRate(ScaledDifficulty);

        public virtual void Setup(MapManager.MissionData currentMission)
        {
            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
            _allEnemies = _allEnemies.Where(e => e.OccurenceList.Contains(currentMission.RegionType)).ToList();
            _currentMission = currentMission;

            var enemyPrefab = GameManager.GetPrefab<EnemyLogic>(PrefabNames.Enemy);
            _enemyPool = PoolHelper.CreatePool(this, enemyPrefab, true);
            GameManager.EnqueueUnloadGameAction(() => ClearAll(_enemyPool));
            PrepareQueue();
            
            _spawn = true;
        }

        public int CountActive => _enemyPool.CountActive;

        protected virtual void Update()
        {
            while (_despawnCount > 0)
            {
                SpawnLogic();
                _despawnCount--;
            }
            
            _progression += Time.deltaTime;
            RunUpdatePoolStack();

            if (!_spawn) return;
            
            if (_waitTimer < _waitBeforeSpawn)
            {
                _waitTimer += Time.deltaTime;
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < MaxTimer) return;
            _timer = 0;

            SpawnLogic();
        }

        protected override PoolObject InvokeQueueUpdate()
        {
            var current = base.InvokeQueueUpdate();
            if (!current.transform.InRange(PlayerManager.PlayerPos, maxDistanceFromPlayer))
            {
                _despawnQueue.Enqueue(current.As<EnemyLogic>().EnemyData);
                _despawnCount++;
                ReleasePoolObject(current);
            }

            return current;
        }

        protected virtual void SpawnLogic()
        {
            if (!PlayerManager.HasInstance() || _enemyPool == null) return;

            MarkerManager.SpawnMarker(this);
        }

        public virtual void StopSpawning()
        {
            _spawn = false;
        }

        public virtual void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            var enemyObj = _enemyPool.Get();
            enemyObj.OnGet(enemy);
            enemyObj.SetPosition(position);
        }

        public virtual void SpawnRandomEntity(Vector2 position)
        {
            SpawnEnemy(GetRandomPoolData() as SoEnemy, position);
        }

        public virtual Color GetMarkerColor()
        {
            return Color.red;
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
            if (!poolObject.Active) return;
            _enemyPool.Release(poolObject as EnemyLogic);
        }

        public override SoPoolObject GetRandomPoolData()
        {
            if (_despawnQueue.Any()) return _despawnQueue.Dequeue();
            
            var pickedDifficulty = GetRandomDifficulty();
            var current = new List<SoEnemy>();
            do
            {
                foreach (var enemy in _allEnemies) 
                    if (enemy.Difficulty == pickedDifficulty) 
                        current.Add(enemy);
                pickedDifficulty--;
            } while (current.Count == 0 && pickedDifficulty > 0);
            return current.RandomElement();
        }
        
        protected virtual int GetRandomDifficulty()
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