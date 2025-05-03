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

            GameManager.OnGMStart += GmStart;
        }

        private void OnDisable()
        {
            GameManager.OnGMStart -= GmStart;
        }

        private void GmStart()
        {
            var markerPrefab = GameManager.GetPrefab<SpawnMarkedEntity>(PrefabNames.SpawnIndicator);
            _markers = PoolHelper.CreatePool(this, markerPrefab, false);
            GameManager.EnqueueUnloadGameAction(() => ClearAll(_markers));
            
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

        public static void SpawnMarker(IUseMarker _spawnerPool)
        {
            Instance.GetPoolObject<SpawnMarkedEntity>().SetReady(_spawnerPool, Instance);
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            if (!poolObject.Active) return;
            _markers.Release(poolObject as SpawnMarkedEntity);
        }
    }
}