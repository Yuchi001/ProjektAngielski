using System;
using PlayerPack;
using UnityEngine;

namespace DifficultyPack
{
    public class DifficultyManager : MonoBehaviour
    {
        [SerializeField] private float enemiesHpScaleBase;
        [SerializeField] private float enemiesHpScalePerSecond;
        [SerializeField] private float spawnRateBase;
        [SerializeField] private float spawnRatePerSeconds;

        private float _progression = 0;

        public static float GetEnemySpawnRate(float time) => Mathf.Lerp(Instance.spawnRateBase, Instance.spawnRateBase + Instance.spawnRatePerSeconds * Instance._progression, Mathf.Clamp01(time));
        public static float EnemyHpScale => Instance.enemiesHpScaleBase + Instance._progression * Instance.enemiesHpScalePerSecond;
        
        private static DifficultyManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void Update()
        {
            if (!PlayerManager.HasInstance() || PlayerManager.CurrentState != PlayerManager.State.ON_MISSION) return;
            _progression += Time.deltaTime;
        }
    }
}