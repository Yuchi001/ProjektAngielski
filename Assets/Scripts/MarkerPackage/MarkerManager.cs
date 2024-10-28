using System.Collections.Generic;
using System.Linq;
using Other;
using Other.Enums;
using UnityEngine;

namespace MarkerPackage
{
    public class MarkerManager : MonoBehaviour
    {
        [SerializeField] private int maxPassiveMarkers = 10;
        [SerializeField] private int maxNegativeMarkers = 50;
        [SerializeField] private GameObject markerPrefab;

        private readonly List<SpawnMarkedEntity> _passiveMarkers = new();
        private readonly List<SpawnMarkedEntity> _negativeMarkers = new();
        
        public static MarkerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        private void Start()
        {
            markerPrefab.SetActive(false);
            
            for (var i = 0; i < maxPassiveMarkers; i++)
            {
                var obj = Instantiate(markerPrefab);
                var marker = obj.GetComponent<SpawnMarkedEntity>();
                marker.SpawnSetup(EEntityType.Positive);
                _passiveMarkers.Add(marker);
            }
            
            for (var i = 0; i < maxNegativeMarkers; i++)
            {
                var obj = Instantiate(markerPrefab);
                var marker = obj.GetComponent<SpawnMarkedEntity>();
                marker.SpawnSetup(EEntityType.Negative);
                _negativeMarkers.Add(marker);
            }
        }
        
        private void OnDisable()
        {
            markerPrefab.SetActive(true);
        }

        public SpawnMarkedEntity GetMarkerFromPool(EEntityType markerType)
        {
            return markerType switch
            {
                EEntityType.Positive => GetValidMarker(_passiveMarkers),
                EEntityType.Negative => GetValidMarker(_negativeMarkers),
                _ => null
            };
        }

        private static SpawnMarkedEntity GetValidMarker(IEnumerable<SpawnMarkedEntity> pool)
        {
            return pool.FirstOrDefault(o => !o.gameObject.activeInHierarchy);
        }
    }
}