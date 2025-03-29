using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using DelaunatorSharp.Unity;
using DelaunatorSharp.Unity.Extensions;
using PlayerPack;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private Vector2 nEnter;
        private Vector2 nExit;
        
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
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
            
            var isInBounds = IsPlayerInBounds();
            if (isInBounds && _wasInBounds) return;

            if (!isInBounds)
            {
                if (_wasInBounds) _newPoints.Add(PlayerPos.ToPoint());

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.A) || Input.GetKeyUp(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.D))
                    _newPoints.Add(PlayerPos.ToPoint());
                _wasInBounds = false;
                return;
            }

            _newPoints.Add(PlayerPos.ToPoint());
            if (IsClockwise(_newPoints)) _newPoints.Reverse();
            //if (_newPoints[0].X < _newPoints[^1].X && _newPoints[0].Y < _newPoints[^1].Y) _newPoints.Reverse();
            //if (_newPoints[0].X > _newPoints[^1].X && _newPoints[0].Y > _newPoints[^1].Y) _newPoints.Reverse();

            var nearestEntry = FindNearestPoint(_newPoints[0].ToVector2());
            var nearestExit = FindNearestPoint(_newPoints[^1].ToVector2());

            nEnter = nearestEntry.point;
            nExit = nearestExit.point;
            
            var temp = points.GetRange(0, nearestEntry.index + 1); 
            temp.AddRange(_newPoints);
            temp.AddRange(points.GetRange(nearestExit.index, points.Count - nearestExit.index));
            points = new List<IPoint>(temp);
            
            _wasInBounds = true;
            var newMesh = CreateMesh(points);
            meshFilter.mesh = newMesh;
            _newPoints.Clear();

            var correctedPoints = new List<IPoint>();
            var bounds = meshFilter.mesh.bounds;
            foreach (var point in points)
            {
                var worldPos = transform.TransformPoint(point.ToVector2());
                var isEdge = Mathf.Approximately(worldPos.x, bounds.min.x) ||
                             Mathf.Approximately(worldPos.x, bounds.max.x) ||
                             Mathf.Approximately(worldPos.y, bounds.min.y) ||
                             Mathf.Approximately(worldPos.y, bounds.max.y);
                if (isEdge) correctedPoints.Add(point);
            }
            points = new List<IPoint>(correctedPoints);
            var correctedMesh = CreateMesh(points);
            meshFilter.mesh = correctedMesh;
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

        private bool IsClockwise(IReadOnlyList<IPoint> list)
        {
            var sum = 0d;
            var n = list.Count;

            for (var i = 0; i < n; i++)
            {
                var current = list[i];
                var next = list[(i + 1) % n];

                sum += (next.X - current.X) * (next.Y + current.Y);
            }

            return sum > 0;
        }

        private void OnDrawGizmos()
        {
            if (meshFilter == null) return;
            Gizmos.color = Color.red;
            var vert = meshFilter.mesh.vertices.ToList();
            for (var i = 0; i < points.Count; i++)
            {
                Gizmos.DrawLine(points[i].ToVector3(), i == points.Count - 1 ? points[0].ToVector3() : points[i+1].ToVector3());
            }
            /*for (var i = 0; i < vert.Count; i++)
            {
                Gizmos.DrawLine(vert[i], i == vert.Count - 1 ? vert[0] : vert[i+1]);
            }*/

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(nEnter, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(nExit, 0.1f);
        }
    }
}