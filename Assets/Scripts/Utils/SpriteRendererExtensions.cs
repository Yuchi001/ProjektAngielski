using UnityEngine;

namespace Utils
{
    public static class SpriteRendererExtensions
    {
        public static Vector2 GetRandomPoint(this SpriteRenderer spriteRenderer)
        {
            var bounds = spriteRenderer.bounds;
            return new Vector2
            {
                x = Random.Range(bounds.min.x, bounds.max.x),
                y = Random.Range(bounds.min.y, bounds.max.y),
            };
        }
    }
}