using System.Collections.Generic;
using System.Linq;
using Managers;
using Other.Enums;
using PoolPack;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Other
{
    public class SpawnMarkedEntity : MonoBehaviour
    {
        [SerializeField] private float spawnTime;
        [SerializeField] private List<EntityColorPair> _colorPairs = new();

        private SpriteRenderer _spriteRenderer;

        private PoolManager _poolManager;

        private float _timer = 0;
        private bool _ready = false;

        #region Setup methods
        public void SpawnSetup(EEntityType entityType)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            var pair = _colorPairs.FirstOrDefault(p => p.entityType == entityType);
            if (pair == default) return;

            _spriteRenderer.color = pair.color;
        }
        
        public SpawnMarkedEntity Setup(PoolManager poolManager)
        {
            _poolManager = poolManager;
            _timer = 0;
            transform.position = GameManager.Instance.MapGenerator.GetRandomPos();
            return this;
        }

        public void SetReady()
        {
            _ready = true;
            gameObject.SetActive(true);
        }

        #endregion

        private void Update()
        {
            if(!_ready) return;
            
            _timer += Time.deltaTime;
            if (_timer < spawnTime) return;

            var poolObj = _poolManager.GetPoolObject();
            poolObj.transform.position = transform.position;
            
            gameObject.SetActive(false);
            _ready = false;
        }
    }

    [System.Serializable]
    public class EntityColorPair
    {
        public EEntityType entityType;
        public Color color;
    }
}