using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Managers;
using Other.Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Other
{
    public class SpawnEntity : MonoBehaviour
    {
        [SerializeField] private float maxXOffset;
        [SerializeField] private float maxYOffset;
        [SerializeField] private float spawnTime;
        [SerializeField] private List<EntityColorPair> _colorPairs = new();

        private Action<GameObject> _spawnAction;
        private GameObject _entityToSpawn;

        private SpriteRenderer _spriteRenderer;
        private Light2D _light2D;

        private float _timer = 0;
        private bool _ready = false;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _light2D = GetComponent<Light2D>();
        }

        #region Setup methods

        public SpawnEntity Setup(GameObject entity, Vector2 position)
        {
            _entityToSpawn = entity;
            transform.position = position;
            return this;
        }
        
        public SpawnEntity Setup(GameObject entity)
        {
            _entityToSpawn = entity;
            transform.position = GetRandomPos();
            return this;
        }

        public SpawnEntity SetSpawnAction(Action<GameObject> spawnAction)
        {
            _spawnAction = spawnAction;
            return this;
        }

        public SpawnEntity SetScale(float scale)
        {
            var scaleVector3 = new Vector3(scale, scale, scale);
            transform.localScale = scaleVector3;
            return this;
        }

        public SpawnEntity SetEntityType(EEntityType entityType)
        {
            var pair = _colorPairs.FirstOrDefault(p => p.entityType == entityType);
            if (pair == default) return this;

            _spriteRenderer.color = pair.color;
            _light2D.color = pair.color;
            return this;
        }

        public void SetReady()
        {
            _ready = true;
        }

        #endregion

        private Vector2 GetRandomPos()
        {
            var pos = Vector2.zero;

            pos.x += Random.Range(-maxXOffset, maxXOffset + 0.1f);
            pos.y += Random.Range(-maxYOffset, maxYOffset + 0.1f);
            
            return pos;
        }

        private void Update()
        {
            if(!_ready) return;
            
            _timer += Time.deltaTime;
            if (_timer < spawnTime) return;

            var spawnedEntity = Instantiate(_entityToSpawn, transform.position, Quaternion.identity);
            _spawnAction?.Invoke(spawnedEntity);
            
            Destroy(gameObject);
            _ready = false;
        }

        public static SpawnEntity InstantiateSpawnEntity()
        {
            var prefab = GameManager.Instance.SpawnEntityPrefab;
            var spawnEntity = Instantiate(prefab);
            return spawnEntity.GetComponent<SpawnEntity>();
        }
    }

    [System.Serializable]
    public class EntityColorPair
    {
        public EEntityType entityType;
        public Color color;
    }
}