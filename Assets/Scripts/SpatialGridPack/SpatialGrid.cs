using System.Collections.Generic;
using UnityEngine;

namespace SpatialGridPack
{
    public class SpatialGrid<T>
    {
        private readonly float cellSize;
        private readonly Dictionary<Vector2Int, HashSet<T>> grid = new();

        public SpatialGrid(float cellSize)
        {
            this.cellSize = cellSize;
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / cellSize),
                Mathf.FloorToInt(worldPos.y / cellSize)
            );
        }

        public void Add(T obj, Vector2 position)
        {
            var cell = WorldToCell(position);
            if (!grid.TryGetValue(cell, out var set))
            {
                set = new HashSet<T>();
                grid[cell] = set;
            }
            set.Add(obj);
        }

        public void Remove(T obj, Vector2 position)
        {
            var cell = WorldToCell(position);
            if (grid.TryGetValue(cell, out var set))
            {
                set.Remove(obj);
                if (set.Count == 0)
                    grid.Remove(cell);
            }
        }

        public void UpdatePosition(T obj, Vector2 oldPos, Vector2 newPos)
        {
            var oldCell = WorldToCell(oldPos);
            var newCell = WorldToCell(newPos);
            if (oldCell != newCell)
            {
                Remove(obj, oldPos);
                Add(obj, newPos);
            }
        }

        public bool GetObjectsNear(Vector2 position, float range, ref List<T> results)
        {
            var center = WorldToCell(position);
            var cellRange = Mathf.CeilToInt(range / cellSize);

            for (var dx = -cellRange; dx <= cellRange; dx++)
            {
                for (var dy = -cellRange; dy <= cellRange; dy++)
                {
                    var cell = new Vector2Int(center.x + dx, center.y + dy);
                    if (!grid.TryGetValue(cell, out var objectsInCell)) continue;
                    results.AddRange(objectsInCell);
                }
            }

            return results.Count > 0;
        }
    }
}