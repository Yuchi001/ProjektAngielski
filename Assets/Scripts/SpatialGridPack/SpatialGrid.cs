using System.Collections.Generic;
using UnityEngine;

namespace SpatialGridPack
{
    public class SpatialGrid<T>
    {
        private readonly float _cellSize;
        private readonly HashSet<T>[,] _grid;
        private readonly Vector2 _worldMin;
        private readonly int _width;
        private readonly int _height;

        public SpatialGrid(float cellSize, Vector2 worldBottomLeft, Vector2 worldTopRight)
        {
            _cellSize = cellSize;
            _worldMin = worldBottomLeft;

            _width = Mathf.CeilToInt((worldTopRight.x - worldBottomLeft.x) / cellSize);
            _height = Mathf.CeilToInt((worldTopRight.y - worldBottomLeft.y) / cellSize);

            _grid = new HashSet<T>[_width, _height];
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    _grid[x, y] = new HashSet<T>();
                }
            }
        }

        private Vector2Int WorldToCell(Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt((worldPos.x - _worldMin.x) / _cellSize),
                Mathf.FloorToInt((worldPos.y - _worldMin.y) / _cellSize)
            );
        }

        private bool TryGetCell(Vector2Int cell, out HashSet<T> set)
        {
            if (cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height)
            {
                set = _grid[cell.x, cell.y];
                return true;
            }
            set = null;
            return false;
        }

        public void Add(T obj, Vector2 position)
        {
            var cell = WorldToCell(position);
            if (TryGetCell(cell, out var set))
            {
                set.Add(obj);
            }
        }

        public void Remove(T obj, Vector2 position)
        {
            var cell = WorldToCell(position);
            if (TryGetCell(cell, out var set))
            {
                set.Remove(obj);
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
            var cellRange = Mathf.CeilToInt(range / _cellSize);

            for (var dx = -cellRange; dx <= cellRange; dx++)
            {
                for (var dy = -cellRange; dy <= cellRange; dy++)
                {
                    var cell = new Vector2Int(center.x + dx, center.y + dy);
                    if (TryGetCell(cell, out var set))
                    {
                        results.AddRange(set);
                    }
                }
            }

            return results.Count > 0;
        }
    }
}