using System.Collections.Generic;
using System.Linq;
using Managers.Base;
using MarkerPackage;
using Other.Enums;
using PoolPack;
using UnityEngine;
using WorldGenerationPack;

namespace Other
{
    public class SpawnMarkedEntity : PoolObject
    {
        [SerializeField] private float spawnTime;
        [SerializeField] private List<EntityColorPair> _colorPairs = new();

        private SpriteRenderer _spriteRenderer;

        private SpawnerBase _poolManager;

        private static readonly string SPAWN_TIMER_ID = "SPAWN_TIMER";

        #region Setup methods

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetReady(EEntityType entityType, SpawnerBase poolManager, MarkerManager markerManager)
        {
            _poolManager = poolManager;
            
            var pair = _colorPairs.FirstOrDefault(p => p.entityType == entityType);
            if (pair == default) return;

            _spriteRenderer.color = pair.color;
            SetTimer(SPAWN_TIMER_ID);
            var randomPosition = ZoneGeneratorManager.GetRandomPos();
            if (!randomPosition.HasValue)
            {
                markerManager.ReleasePoolObject(this);
                return;
            }
            
            transform.position = randomPosition.Value;
            OnGet(null);
        }

        #endregion

        public override void InvokeUpdate(float realDeltaTime)
        {
            if (CheckTimer(SPAWN_TIMER_ID) < spawnTime) return;

            _poolManager.SpawnRandomEntity(transform.position);
            
            MarkerManager.Instance.ReleasePoolObject(this);
            SetTimer(SPAWN_TIMER_ID);
        }
    }

    [System.Serializable]
    public class EntityColorPair
    {
        public EEntityType entityType;
        public Color color;
    }
}