using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorPack
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Vector2 scalingMinMax;
        private void Start()
        {
            var random = new Vector2();
            random.x = Random.Range(scalingMinMax.x, scalingMinMax.y + 0.1f);
            random.y = Random.Range(scalingMinMax.x, scalingMinMax.y + 0.1f);
        }
    }
}