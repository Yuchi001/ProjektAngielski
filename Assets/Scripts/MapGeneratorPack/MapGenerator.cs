using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorPack
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private float chestSpawnChance = 10f;
        [SerializeField] private float maxDistance = 5;
        [SerializeField] private GameObject zonePrefab;
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private float zonesPerSecond = 0.1f;
        [SerializeField] private float checkPlayerDistancePerSecond = 0.5f;
        
        private List<Zone> _zones = new();
        
        private bool _generate = false;

        private float _playerDistanceTimer = 0;
        private float _zoneSpawnTimer = 0;

        private List<Zone> CloseZones
        {
            get { return _currentZones ??= GetCloseZone(); }
            set => _currentZones = value;
        }
        private List<Zone> _currentZones = null;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);
            Generate();
        }

        public void Generate()
        {
            _zones.Add(SpawnZone());
            _generate = true;
        }

        private void Update()
        {
            if (!_generate) return;

            _playerDistanceTimer += Time.deltaTime;
            _zoneSpawnTimer += Time.deltaTime;

            if (_playerDistanceTimer > 1f / checkPlayerDistancePerSecond)
            {
                CloseZones = GetCloseZone();
                _playerDistanceTimer = 0;
            }

            if (_zoneSpawnTimer > 1f / zonesPerSecond)
            {
                SpawnZone();
                _zoneSpawnTimer = 0;

                if (Random.Range(0f, 101f) > chestSpawnChance) return;
                
                Instantiate(chestPrefab, _zones[Random.Range(0, _zones.Count)].GetRandomPos(), Quaternion.identity);
            }
        }

        public Vector2 GetRandomPos()
        {
            if (CloseZones.Count == 0) return Vector2.zero;
            return CloseZones[Random.Range(0, CloseZones.Count)].GetRandomPos();
        }

        private List<Zone> GetCloseZone()
        {
            var playerPos = PlayerManager.Instance.transform.position;
            return _zones.Where(z => Vector2.Distance(playerPos, z.transform.position) < maxDistance).ToList();
        }

        private Zone SpawnZone()
        {
            var obj = Instantiate(zonePrefab);
            var zone = obj.GetComponent<Zone>();
            zone.Setup(CloseZones.Count > 0 ? CloseZones[Random.Range(0, CloseZones.Count)] : null);
            _zones.Add(zone);
            return zone;
        }

        public bool ContainsEntity(Vector2 position)
        {
            return _zones.Any(z => z.Contains(position));
        }
    }
}