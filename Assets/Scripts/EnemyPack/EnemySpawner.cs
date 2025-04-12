using System.Collections.Generic;
using System.Linq;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using Managers;
using Managers.Base;
using Managers.Enums;
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

        private Queue<SoEnemy> _despawnQueue = new();

        private List<SoEnemy> _allEnemies = new();
        
        private float _difficultyTimer = 0;

        public delegate void EnemyDieDelegate(EnemyLogic enemyLogic);
        public static event EnemyDieDelegate OnEnemyDie;

        private ObjectPool<EnemyLogic> _enemyPool;
 
        public int DeadEnemies { get; private set; }

        private float EnemySpawnRate => enemySpawnRate + 1f * enemySpawnRateMultiplierPerKill * DeadEnemies;
        protected override float MaxTimer => 1f / (enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * EnemySpawnRate);
        public float EnemiesHpScale => enemiesHpScale + Mathf.Pow(1f + DeadEnemies * enemiesHpScaleMultiplierPerKill, 2) - 1;

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
        }

        protected override void Update()
        {
            RunUpdatePoolStack();
            
            base.Update();

            if (_state == ESpawnerState.Stop) return;
            _difficultyTimer += Time.deltaTime;
        }

        protected override PoolObject InvokeUpdate()
        {
            var current = base.InvokeUpdate();
            if (!current.transform.InRange(PlayerManager.PlayerPos, 10))
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
            
            var sum = 55; // 10 + 9 + 8 ...
            var randomInt = Random.Range(0, sum + 1) * (1 - Mathf.Clamp(_difficultyTimer / maximumDifficultyTimeCap, 0, 1));
            var difficultyList = new List<int> { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            difficultyList.Sort();
            var pickedDifficulty = 0;
            foreach (var diff in difficultyList)
            {
                if (randomInt <= diff)
                {
                    pickedDifficulty = diff;
                    break;
                }

                randomInt -= diff;
            }

            var enemies = GetValidEnemies();
            var soEnemies = enemies as SoEnemy[] ?? enemies.ToArray();
            return soEnemies.ElementAt(Random.Range(0, soEnemies.Length));

            IEnumerable<SoEnemy> GetValidEnemies()
            {
                var current = new List<SoEnemy>();
                foreach (var enemy in _allEnemies)
                    if (enemy.Difficulty <= pickedDifficulty) current.Add(enemy);
                return current.Count > 0 ? current : new List<SoEnemy> {_allEnemies[0]};
            }
        }
    }
}