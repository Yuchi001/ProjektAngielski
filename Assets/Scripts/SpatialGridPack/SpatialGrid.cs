using System.Collections.Generic;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace SpatialGridPack
{
    public class SpatialGrid<T> where T: PoolObject
    {
        private readonly float _cellSize;
        private readonly HashSet<int>[,] _grid;
        private readonly Vector2 _worldMin;
        private readonly int _width;
        private readonly int _height;

        private readonly Dictionary<int, T> _activeObjects = new();

        public SpatialGrid(float cellSize, Vector2 worldBottomLeft, Vector2 worldTopRight)
        {
            _cellSize = cellSize;
            _worldMin = worldBottomLeft;

            _width = Mathf.CeilToInt((worldTopRight.x - worldBottomLeft.x) / cellSize);
            _height = Mathf.CeilToInt((worldTopRight.y - worldBottomLeft.y) / cellSize);

            _grid = new HashSet<int>[_width, _height];
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    _grid[x, y] = new HashSet<int>();
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

        private bool TryGetCell(Vector2Int cell, out HashSet<int> set)
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
            if (!TryGetCell(cell, out var set)) return;
            
            var id = obj.GetInstanceID();
            set.Add(id);
            _activeObjects.Add(id, obj);
        }

        public void Remove(T obj, Vector2 position)
        {
            var cell = WorldToCell(position);
            if (!TryGetCell(cell, out var set)) return;
            
            var id = obj.GetInstanceID();
            set.Remove(id);
            _activeObjects.Remove(id);
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

            var minX = Mathf.Max(0, center.x - cellRange);
            var maxX = Mathf.Min(_width - 1, center.x + cellRange);
            var minY = Mathf.Max(0, center.y - cellRange);
            var maxY = Mathf.Min(_height - 1, center.y + cellRange);

            var estimatedTotal = results.Count;
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    estimatedTotal += _grid[x, y].Count;
                }
            }

            if (results.Capacity < estimatedTotal)
                results.Capacity = estimatedTotal;

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    foreach (var id in _grid[x, y])
                    {
                        if (_activeObjects.TryGetValue(id, out var o)) results.Add(o);
                    }
                }
            }

            return results.Count > 0;
        }
    }
}