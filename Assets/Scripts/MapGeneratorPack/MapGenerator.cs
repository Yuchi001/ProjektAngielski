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
                if (_wasInBounds) points.Add(FindNearestPoint(PlayerPos).ToPoint());
                
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.A) || Input.GetKeyUp(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.D))
                {
                }

                points.Add(PlayerPos.ToPoint());
                _wasInBounds = false;
                return;
            }

            points.Add(FindNearestPoint(PlayerPos).ToPoint());
            _wasInBounds = true;
            var newMesh = CreateMesh(points);
            var combine = new CombineInstance[2];
            combine[0].mesh = meshFilter.mesh;
            combine[0].transform = meshFilter.transform.localToWorldMatrix;
            combine[1].mesh = newMesh;
            combine[1].transform = Matrix4x4.identity;
            var resultMesh = new Mesh();
            resultMesh.CombineMeshes(combine);
            resultMesh.RecalculateNormals();
            resultMesh.RecalculateBounds();
            resultMesh.Optimize();
            meshFilter.mesh = resultMesh;
            points.Clear();
        }

        private Vector3 FindNearestPoint(Vector3 point)
        {
            var vertices = meshFilter.mesh.vertices;

            vertices = vertices.Select(v => meshFilter.transform.TransformPoint(v)).ToArray();

            return vertices.OrderBy(v => Vector3.SqrMagnitude(v - point)).First();
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