using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers.Base;
using Managers.Enums;
using MapPack;
using UnityEngine;

namespace Managers
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<SpawnerBase> spawners = new();

        public EnemySpawner EnemySpawner { get; private set; }

        private void Awake()
        {
            GameManager.OnStartRun += BeginSpawn;
            
            var enemySpawner = spawners.FirstOrDefault(s => s.GetType() == typeof(EnemySpawner));
            if (enemySpawner == default) return;
            EnemySpawner = enemySpawner as EnemySpawner;
        }

        private void OnDisable()
        {
            GameManager.OnStartRun -= BeginSpawn;
        }

        public void BeginSpawn(MapManager.MissionData missionData)
        {
            spawners.ForEach(s => s.SetState(ESpawnerState.Spawn));
        }
    }
}