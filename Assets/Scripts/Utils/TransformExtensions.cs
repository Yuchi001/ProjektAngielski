using UnityEngine;

namespace Utils
{
    public static class TransformExtensions
    {
        public static void AdjustForPivot(this Transform transform, SpriteRenderer renderer)
        {
            var position = transform.position;
            var centerOffset = renderer.bounds.center - position;
            position -= centerOffset;
            transform.position = position;
        }
    }
}