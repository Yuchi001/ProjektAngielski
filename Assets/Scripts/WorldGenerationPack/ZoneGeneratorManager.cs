﻿using System.Collections.Generic;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.SO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorldGenerationPack
{
    public class ZoneGeneratorManager : MonoBehaviour
    {
        #region Singleton

        private static ZoneGeneratorManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            _zonePrefab = GameManager.GetPrefab<Zone>(PrefabNames.Zone);
            _zoneDict = new Dictionary<string, Zone>();
        }
        #endregion

        [SerializeField] private float zoneEntityRange;
        
        private Zone _zonePrefab;

        private Dictionary<string, Zone> _zoneDict;

        public static bool HasMainZone() => Instance._zoneDict.Keys.Count > 0;

        public static bool ContainsEntity(Vector2 pos)
        {
            foreach (var zone in Instance._zoneDict.Values)
            {
                if (zone == null || !zone.ContainsEntity(pos)) continue;
                return true;
            }

            return false;
        }

        public static Vector2? GetRandomPos()
        {
            var playerPos = PlayerManager.PlayerPos;
            var availableZones = new List<Zone>();

            foreach (var zone in Instance._zoneDict.Values)
            {
                if (!zone.InRange(playerPos, Instance.zoneEntityRange)) continue;
                availableZones.Add(zone);
            }

            if (availableZones.Count == 0) return null;

            var randomIndex = Random.Range(0, availableZones.Count);
            return availableZones[randomIndex].GetRandomPos();
        }

        public static void ExpandZone(string key, float percentage)
        {
            var dict = Instance._zoneDict;
            if (!dict.ContainsKey(key)) return;
            
            dict[key].Resize(percentage);
        }

        public static void GenerateZone(Vector2 position, string key, float scale, bool withAnim = true)
        {
            var zone = Instantiate(Instance._zonePrefab, position, Quaternion.identity, Instance.transform);
            zone.SetSize(scale, withAnim);
            Instance._zoneDict.Add(key, zone);
        }
    }
}