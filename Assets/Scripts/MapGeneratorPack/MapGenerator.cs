using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using DelaunatorSharp.Unity;
using DelaunatorSharp.Unity.Extensions;
using PlayerPack;
using UnityEngine;
using Utils;

//TODO: uzupełnij
namespace MapGeneratorPack
{
    [RequireComponent(typeof(MeshFilter))]
    public class MapGenerator : MonoBehaviour
    {
        private List<IPoint> points = new List<IPoint>();
        private List<IPoint> _newPoints = new List<IPoint>();
        private GameObject meshObject;
        
        private MeshFilter meshFilter;

        private bool _wasInBounds = true;

        private static Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;
        
        #region Singleton

        private static MapGenerator Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            meshFilter = GetComponent<MeshFilter>();
        }

        #endregion

        public static bool ContainsEntity(Vector2 pos)
        {
            return false;
        }

        public static Vector2 GetRandomPos()
        {
            return Vector2.zero;
        }


        private void Start()
        {
            points = new List<IPoint>(){new Point(-1, 1), new Point(1, 1), new Point(1, -1), new Point(-1, -1)}.ToList();
            meshFilter.mesh = CreateMesh(points);
        }
        
        private bool IsPlayerInBounds()
        {
            if (meshFilter == null || meshFilter.mesh == null) return false;
            return meshFilter.mesh.bounds.Contains(transform.InverseTransformPoint(PlayerPos));
        }

        private void Update()
        {
            var isInBounds = IsPlayerInBounds();
            if (isInBounds && _wasInBounds) return;

            if (!isInBounds)
            {
                //if (_wasInBounds) _newPoints.Add(PlayerPos.ToPoint());

                _newPoints.Add(PlayerPos.ToPoint());
                _wasInBounds = false;
                return;
            }

            _newPoints.Add(PlayerPos.ToPoint());

            var nearestEntry = FindNearestPoint(_newPoints[0].ToVector2());
            var nearestExit = FindNearestPoint(_newPoints[^1].ToVector2());

            var temp = points.GetRange(0, nearestEntry.index + 1);
            temp.AddRange(_newPoints);
            temp.AddRange(points.GetRange(nearestExit.index, points.Count - nearestExit.index));
            points = new List<IPoint>(temp);
            
            _wasInBounds = true;
            var newMesh = CreateMesh(points);
            meshFilter.mesh = newMesh;
            _newPoints.Clear();
        }

        private (int index, Vector3 point) FindNearestPoint(Vector3 point)
        {
            var vertices = meshFilter.mesh.vertices;

            (int index,Vector3 point)[] arr = vertices.Select((v, index) => (index, meshFilter.transform.TransformPoint(v))).ToArray();

            return arr.OrderBy(v => Vector3.SqrMagnitude(v.point - point)).First();
        }
        

        private static Mesh CreateMesh(List<IPoint> points)
        {
            var delaunator = new Delaunator(points.ToArray());
            var mesh = new Mesh
            {
                vertices = delaunator.Points.ToVectors3(),
                triangles = delaunator.Triangles
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
    }
}