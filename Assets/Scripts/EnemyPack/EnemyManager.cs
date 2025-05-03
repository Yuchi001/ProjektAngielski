using System.Collections.Generic;
using EnemyPack.SO;
using Managers;
using MapPack;
using PoolPack;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnemyPack
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager Instance { get; set; }
        private static EnemySpawner _currentSpawner;

        public delegate void EnemyDeathDelegate(EnemyLogic enemyLogic);
        public static EnemyDeathDelegate OnEnemyDeath;

        public static bool Ready => _currentSpawner != null;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public static void SetCurrentSpawner(MapManager.MissionData missionData)
        {
            _currentSpawner = Instantiate(missionData.SpawnerPrefab);
            _currentSpawner.Setup(missionData);
            var scene = SceneManager.GetSceneByBuildIndex((int)GameManager.EScene.GAME);
            SceneManager.MoveGameObjectToScene(_currentSpawner.gameObject, scene);
        }

        public static void RegisterEnemyDeath(EnemyLogic logic)
        {
            OnEnemyDeath?.Invoke(logic);
        }

        public static void StopSpawning()
        {
            _currentSpawner.StopSpawning();
        }

        public static void SpawnEnemy(SoEnemy enemy, Vector2 position)
        {
            _currentSpawner.SpawnEnemy(enemy, position);
        }

        public static List<PoolObject> GetActiveEnemies() => _currentSpawner.GetActiveEnemies();
    }
}