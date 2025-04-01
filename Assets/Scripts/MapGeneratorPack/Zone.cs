using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorPack
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Zone : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool ContainsEntity(Vector2 pos)
        {
            return spriteRenderer.bounds.Contains(pos);
        }

        public Vector2 GetRandomPos()
        {
            var bounds = spriteRenderer.bounds;
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector2(x, y);
        }

        public bool InRange(Vector2 pos, float range)
        {
            return ContainsEntity(pos) || Vector2.Distance(transform.position, pos) <= range;
        }

        public void Resize(float percentage)
        {
            percentage *= 2;
            var randomDivision = Random.Range(0.01f, 0.99f);
            var scale = transform.localScale;
            scale.x *= 1 + percentage * randomDivision;
            scale.y *= 1 + percentage * (1 - randomDivision);
            transform.LeanScale(scale, 0.3f).setEaseInBack().setEaseOutBack();
        }

        public void SetSize(float scale, bool anim)
        {
            scale *= 2;
            var randomDivision = Random.Range(0.4f, 0.6f);
            var vec = new Vector3
            {
                x = scale * randomDivision,
                y = scale * (1 - randomDivision)
            };
            if (!anim)
            {
                transform.localScale = vec;
                return;
            }

            transform.LeanScale(vec, 0.3f).setEaseInBack().setEaseOutBack();
        }
    }
}