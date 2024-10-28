using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Base;
using Other.Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Other
{
    public class SpawnMarkedEntity : MonoBehaviour
    {
        [SerializeField] private float spawnTime;
        [SerializeField] private List<EntityColorPair> _colorPairs = new();

        private SpriteRenderer _spriteRenderer;
        private Light2D _light2D;
        
        private EntityBase _entity;
        private SoEntityBase _data;

        private float _timer = 0;
        private bool _ready = false;

        #region Setup methods
        public void SpawnSetup(EEntityType entityType)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _light2D = GetComponent<Light2D>();
            
            var pair = _colorPairs.FirstOrDefault(p => p.entityType == entityType);
            if (pair == default) return;

            _spriteRenderer.color = pair.color;
            _light2D.color = pair.color;
        }
        
        public SpawnMarkedEntity Setup(EntityBase entity, SoEntityBase data)
        {
            _timer = 0;
            _entity = entity;
            _data = data;
            transform.position = GameManager.Instance.MapGenerator.GetRandomPos();
            return this;
        }

        public SpawnMarkedEntity SetScale(float scale)
        {
            var scaleVector3 = new Vector3(scale, scale, scale);
            transform.localScale = scaleVector3;
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
            
            _entity.Setup(_data);
            _entity.transform.position = transform.position;
            
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