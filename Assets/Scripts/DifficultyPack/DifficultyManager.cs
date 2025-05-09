﻿using GameLoaderPack;
using MapPack;
using PlayerPack;
using UnityEngine;
using Utils;

namespace DifficultyPack
{
    public class DifficultyManager : MonoBehaviour, IMissionDependentInstance
    {
        [SerializeField] private float enemiesHpScaleBase;
        [SerializeField] private float enemiesHpScalePerSecond;
        [SerializeField] private MinMax spawnRate;
        [SerializeField] private float spawnRatePerSeconds;

        private float _progression = 0;
        private MapManager.MissionData _currentMission;
        public static float EnemyHpScale => (Instance.enemiesHpScaleBase + Instance._progression * Instance.enemiesHpScalePerSecond) * 
                                            ((int)Instance._currentMission.Difficulty + 1);
        
        private static DifficultyManager Instance { get; set; }
        
        public void Init(MapManager.MissionData missionData)
        {
            _currentMission = missionData;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        
        public static float GetEnemySpawnRate(float time)
        {
            var spawnRateBounds = Instance.spawnRate;
            var maxSpawnRate = Instance.spawnRate.Min + Instance.spawnRatePerSeconds * Instance._progression;
            var clampedMaxSpawnRate = Mathf.Clamp(maxSpawnRate, spawnRateBounds.Min, spawnRateBounds.Max);
            return Mathf.Lerp(Instance.spawnRate.Min, clampedMaxSpawnRate, time);
        }

        private void Update()
        {
            if (!PlayerManager.HasInstance() || PlayerManager.CurrentState != PlayerManager.State.ON_MISSION) return;
            _progression += Time.deltaTime;
        }
    }
}