using System.Collections.Generic;
using EnemyPack.SO;
using Managers;
using MapPack;
using PoolPack;
using SpatialGridPack;
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

        private SpatialGrid<EnemyLogic> _activeEnemies;

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
            var offset = missionData.WorldSize / 2;
            Instance._activeEnemies = new SpatialGrid<EnemyLogic>(1, -offset, offset);
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
        
        public static void AddPos(EnemyLogic enemyLogic)
        {
            Instance._activeEnemies.Add(enemyLogic, enemyLogic.transform.position);
        }

        public static void UpdatePos(EnemyLogic enemyLogic, Vector2 lastPos)
        {
            Instance._activeEnemies.UpdatePosition(enemyLogic, lastPos, enemyLogic.transform.position);
        }

        public static void RemovePos(EnemyLogic enemyLogic)
        {
            Instance._activeEnemies?.Remove(enemyLogic, enemyLogic.transform.position);
        }

        public static int CountActive => _currentSpawner.CountActive;

        public static bool GetNearbyEnemies(Vector2 position, float range, ref List<EnemyLogic> enemies)
        {
            return Instance._activeEnemies.GetObjectsNear(position, range, ref enemies);
        }
    }
}