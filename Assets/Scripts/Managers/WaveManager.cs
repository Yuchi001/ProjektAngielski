using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers.Base;
using Managers.Enums;
using UnityEngine;

namespace Managers
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<SpawnerBase> spawners = new();

        private EnemySpawner _enemySpawner;

        public EnemySpawner EnemySpawner => _enemySpawner;

        private void Awake()
        {
            var enemySpawner = spawners.FirstOrDefault(s => s.GetType() == typeof(EnemySpawner));
            if (enemySpawner == default) return;
            _enemySpawner = enemySpawner as EnemySpawner;
        }

        public void BeginSpawn()
        {
            spawners.ForEach(s => s.SetState(ESpawnerState.Spawn));
        }
    }
}