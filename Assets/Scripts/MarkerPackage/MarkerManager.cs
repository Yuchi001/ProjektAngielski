using Managers;
using Managers.Base;
using Managers.Other;
using Other;
using Other.Enums;
using PoolPack;
using UnityEngine.Pool;

namespace MarkerPackage
{
    public class MarkerManager : PoolManager
    {
        private ObjectPool<SpawnMarkedEntity> _markers;
        private ObjectPool<SpawnMarkedEntity> _negativeMarkers;
        
        public static MarkerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        private void Start()
        {
            var markerPrefab = GameManager.Instance.GetPrefab<SpawnMarkedEntity>(PrefabNames.SpawnIndicator);
            _markers = PoolHelper.CreatePool(this, markerPrefab, false);
            
            PrepareQueue();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        protected override T GetPoolObject<T>()
        {
            return _markers.Get() as T;
        }

        public static void SpawnMarker(SpawnerBase _spawnerPool, EEntityType entityType)
        {
            Instance.GetPoolObject<SpawnMarkedEntity>().SetReady(entityType, _spawnerPool, Instance);
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _markers.Release(poolObject as SpawnMarkedEntity);
        }
    }
}