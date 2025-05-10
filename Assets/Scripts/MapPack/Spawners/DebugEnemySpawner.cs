using System;
using EnemyPack;
using EnemyPack.SO;
using Managers;
using PlayerPack;
using StructurePack.SO;
using UnityEngine;
using WorldGenerationPack;

namespace MapPack.Spawners
{
    public class DebugEnemySpawner : EnemySpawner
    {
        private SoEnemy _dummy;
        private SoChestStructure _chest;
        private void Awake()
        {
            _dummy = Resources.Load<SoEnemy>("Enemies/DEBUG/Dummy");
            _chest = Resources.Load<SoChestStructure>("Structures/Chest");
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) SpawnEnemy(_dummy, PlayerManager.PlayerPos);
            if (Input.GetKeyDown(KeyCode.C)) StructureManager.SpawnStructure(_chest, PlayerManager.PlayerPos, GameManager.EScene.GAME);
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (var e in ActiveObjects)
                {
                    if (e is EnemyLogic enemyLogic) enemyLogic.GetDamaged(int.MaxValue);
                }
            }
        }
    }
}