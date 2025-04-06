using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using GameLoaderPack;
using Managers.Base;
using Managers.Enums;
using MapPack;
using UnityEngine;

namespace Managers
{
    public class WaveManager : MonoBehaviour, IMissionDependentInstance
    {
        [SerializeField] private List<SpawnerBase> spawners = new();

        public EnemySpawner EnemySpawner { get; private set; }

        private void Awake()
        {
            var enemySpawner = spawners.FirstOrDefault(s => s.GetType() == typeof(EnemySpawner));
            if (enemySpawner == default) return;
            EnemySpawner = enemySpawner as EnemySpawner;
        }

        public void Init(MapManager.MissionData missionData)
        {
            foreach (var spawner in spawners)
            {
                spawner.Init(missionData);
                spawner.SetState(ESpawnerState.Spawn);
            }
        }
    }
}