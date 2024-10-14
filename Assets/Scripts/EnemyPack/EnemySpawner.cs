using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using ExpPackage.Enums;
using Managers;
using Managers.Base;
using Managers.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    public class EnemySpawner : SpawnerBase
    {
        [SerializeField] private float enemiesHpScaleMultiplierPerKill = 0.001f;
        [SerializeField] private float enemiesHpScale = 1;
        [SerializeField] private int maxEnemiesCount = 300;
        [SerializeField] private GameObject enemyPrefab;
        [Space(10)]
        [SerializeField] private AnimationCurve enemySpawnRateCurve;
        [SerializeField] private float enemySpawnRateMultiplierPerKill = 0.001f;
        [SerializeField] private float enemySpawnRate;
        [Space(10)]
        [SerializeField, Tooltip("In seconds")] private int maximumDifficultyTimeCap = 3600;
        [SerializeField] private List<GemRarityTuple> gemRarityList;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();
        
        private float _difficultyTimer = 0;

        public delegate void EnemyDieDelegate(EnemyLogic enemyLogic);
        public static event EnemyDieDelegate OnEnemyDie; 
 
        public int DeadEnemies { get; private set; }

        public void IncrementDeadEnemies(EnemyLogic enemyLogic)
        {
            OnEnemyDie?.Invoke(enemyLogic);
            DeadEnemies++;
        }

        private float EnemySpawnRate => enemySpawnRate + 1f * enemySpawnRateMultiplierPerKill * DeadEnemies;
        protected override float MaxTimer => 1f / (enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * EnemySpawnRate);
        public float EnemiesHpScale => enemiesHpScale + 1f * DeadEnemies * enemiesHpScaleMultiplierPerKill;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.MapGenerator != null);
            
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
        }

        protected override void Update()
        {
            base.Update();

            if (_state == ESpawnerState.Stop) return;
            _difficultyTimer += Time.deltaTime;
        }

        protected override void SpawnLogic()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemiesCount || PlayerManager == null) return;
            
            var enemySo = GetEnemy(false);
            
            SpawnEntity.InstantiateSpawnEntity()
                .Setup(enemyPrefab)
                .SetEntityType(EEntityType.Negative)
                .SetScale(enemySo.BodyScale)
                .SetSpawnAction((spawnedObj) => SetupEnemy(spawnedObj, enemySo))
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

        private void SetupEnemy(GameObject enemyObj, SoEnemy enemy)
        {
            var enemyScript = enemyObj.GetComponent<EnemyLogic>();
            var scale = enemy.BodyScale;
            enemyObj.transform.localScale = new Vector3(scale, scale, scale);
            
            enemyScript.Setup(enemy, PlayerManager.transform, this);
        }

        public void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            var enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
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