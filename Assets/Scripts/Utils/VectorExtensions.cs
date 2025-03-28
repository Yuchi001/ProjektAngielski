using DelaunatorSharp;
using UnityEngine;

namespace Utils
{
    public static class VectorExtensions
    {
        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point(vector2.x, vector2.y);
        }

        public static Point ToPoint(this Vector3 vector3)
        {
            return new Point(vector3.x, vector3.y);
        }
    }
}