using System.Collections.Generic;
using System.Linq;
using EnemyPack.SO;
using ExpPackage.Enums;
using Managers;
using Managers.Base;
using Other;
using Other.Enums;
using PlayerPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    public class EnemySpawner : SpawnerBase
    {
        [SerializeField] private float enemiesHpScale = 1;
        [SerializeField] private int maxEnemiesCount = 300;
        [SerializeField] private GameObject enemyPrefab;
        [Space(10)]
        [SerializeField] private AnimationCurve enemySpawnRateCurve;
        [SerializeField] private float enemySpawnRate;
        [Space(10)]
        [SerializeField, Tooltip("In seconds")] private int maximumDifficultyTimeCap = 3600;
        [SerializeField] private List<GemRarityTuple> gemRarityList;
        private PlayerManager PlayerManager => GameManager.Instance.CurrentPlayer;

        private List<SoEnemy> _allEnemies = new();
        
        private float _difficultyTimer = 0;
        
        public float EnemiesHpScale => enemiesHpScale;
        protected override float MaxTimer => 1f / (enemySpawnRateCurve.Evaluate(_difficultyTimer / maximumDifficultyTimeCap) * enemySpawnRate);
        public int DeadEnemies { get; set; } = 0;
        
        private void Start()
        {
            DeadEnemies = 0;

            _allEnemies = Resources.LoadAll<SoEnemy>("Enemies").Select(Instantiate).ToList();
        }

        protected override void Update()
        {
            _difficultyTimer += Time.deltaTime;
            base.Update();
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
    }

    [System.Serializable]
    public class GemRarityTuple
    {
        public EExpGemType gemType;
        public int weight;
    }
}