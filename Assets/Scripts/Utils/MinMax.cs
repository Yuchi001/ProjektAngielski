using UnityEngine;
using UnityEngine.Serialization;

namespace Utils
{
    [System.Serializable]
    public class MinMax
    {
        [SerializeField] private float min;
        [SerializeField] private float max;

        public int RandomInt()
        {
            return Random.Range((int)min, (int)max + 1);
        }

        public float RandomFloat()
        {
            return Random.Range(min, max);
        }

        public float Min => min;
        public float Max => max;
    }
}