using System.Collections.Generic;
using MapGeneratorPack.Enums;
using StagePack;
using UnityEngine;
using Utils;

namespace MapPack
{
    public class Region : MonoBehaviour
    {
        [SerializeField] private SoRegion regionData;
        [SerializeField] private List<SpriteRenderer> regionAreas;

        public SoRegion RegionData => regionData;

        public Vector2 GetRandomRegionPoint()
        {
            var randomElement = regionAreas.RandomElement();
            return randomElement.GetRandomPoint();
        }
    }
}