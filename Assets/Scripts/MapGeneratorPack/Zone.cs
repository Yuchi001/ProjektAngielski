using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorPack
{
    public class Zone : MonoBehaviour
    {
        [SerializeField] private float tileLength;
        [SerializeField] private Vector2Int scalingMinMax;
        [SerializeField] private GameObject saveZone;

        private SpriteRenderer _spriteRenderer;

        public void Setup(Zone parentZone)
        {
            _spriteRenderer = saveZone.GetComponent<SpriteRenderer>();
            var random = new Vector2
            {
                x = Random.Range(scalingMinMax.x, scalingMinMax.y + 1),
                y = Random.Range(scalingMinMax.x, scalingMinMax.y + 1)
            };
            random *= tileLength;

            transform.position = parentZone?.GetRandomPos() ?? Vector2.zero;
            transform.localScale = random;
        }

        public Vector2 GetRandomPos()
        {
            var bounds = _spriteRenderer.bounds;
            return new Vector2
            {
                x = Random.Range(bounds.min.x, bounds.max.x),
                y = Random.Range(bounds.min.y, bounds.max.y)
            };
        }

        public bool Contains(Vector2 position)
        {
            return _spriteRenderer.bounds.Contains(position);
        }
    }
}