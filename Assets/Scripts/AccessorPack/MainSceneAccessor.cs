using System;
using EnemyPack;
using Managers;
using UnityEngine;

namespace AccessorPack
{
    // TODO: REMOVE THIS
    public class MainSceneAccessor : MonoBehaviour
    {
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private EnemySpawner enemySpawner;

        public static WaveManager WaveManager => Instance.waveManager;
        public static EnemySpawner EnemySpawner => Instance.enemySpawner;

        private static MainSceneAccessor Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
    }
}