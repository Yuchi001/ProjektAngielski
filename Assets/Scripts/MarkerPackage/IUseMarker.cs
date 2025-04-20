using UnityEngine;

namespace MarkerPackage
{
    public interface IUseMarker
    {
        public void SpawnRandomEntity(Vector2 spawnPos);

        public Color GetMarkerColor();
    }
}