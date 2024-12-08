using System;
using Managers;
using Managers.Other;
using Other;
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
            var markerPrefab = GameManager.Instance.GetPrefab(PrefabNames.SpawnIndicator);
            _markers = PoolHelper.CreatePool<SpawnMarkedEntity>(this, markerPrefab);
            
            PrepareQueue();
        }

        private void Update()
        {
            RunUpdatePoolStack();
        }

        public override PoolObject GetPoolObject()
        {
            return _markers.Get();
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _markers.Release(poolObject as SpawnMarkedEntity);
        }

        public override SoEntityBase GetRandomPoolData()
        {
            return null; // Not needed
        }
    }
}