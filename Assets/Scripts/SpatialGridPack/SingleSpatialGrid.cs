using UnityEngine;

namespace SpatialGridPack
{
    public class SingleSpatialGrid<T>
    {
        private readonly float cellSize;

        private Vector2Int currentCell;

        public SingleSpatialGrid(float cellSize, Vector3 position)
        {
            this.cellSize = cellSize;
            currentCell = WorldToCell(position);
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / cellSize),
                Mathf.FloorToInt(worldPos.y / cellSize)
            );
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            currentCell = WorldToCell(newPosition);
        }

        public bool IsNear(Vector2 position, float range)
        {
            var center = WorldToCell(position);
            var cellRange = Mathf.CeilToInt(range / cellSize);

            return Mathf.Abs(center.x - currentCell.x) <= cellRange &&
                   Mathf.Abs(center.y - currentCell.y) <= cellRange;
        }
    }
}