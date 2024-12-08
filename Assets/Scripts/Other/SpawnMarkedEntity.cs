using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MarkerPackage;
using Other.Enums;
using PoolPack;
using UnityEngine;

namespace Other
{
    public class SpawnMarkedEntity : PoolObject
    {
        [SerializeField] private float spawnTime;
        [SerializeField] private List<EntityColorPair> _colorPairs = new();

        private SpriteRenderer _spriteRenderer;

        private PoolManager _poolManager;

        private static readonly string SPAWN_TIMER_ID = "SPAWN_TIMER";

        #region Setup methods

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnGet(SoEntityBase so)
        {
            base.OnGet(so);
            
            transform.position = GameManager.Instance.MapGenerator.GetRandomPos();
        }

        public void SetReady(EEntityType entityType, PoolManager poolManager)
        {
            _poolManager = poolManager;
            
            var pair = _colorPairs.FirstOrDefault(p => p.entityType == entityType);
            if (pair == default) return;

            _spriteRenderer.color = pair.color;
            SetTimer(SPAWN_TIMER_ID);
        }

        #endregion

        public override void InvokeUpdate()
        {
            if (CheckTimer(SPAWN_TIMER_ID) < spawnTime) return;

            var poolObj = _poolManager.GetPoolObject();
            poolObj.transform.position = transform.position;
            
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