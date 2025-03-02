using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack;
using StructurePack.SO;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO: uzupełnij
namespace MapGeneratorPack
{
    [RequireComponent(typeof(MeshFilter))]
    public class MapGenerator : MonoBehaviour
    {
        private List<SoStructure> _structures = new();

        private List<Vector3> _vertices;
        private List<int> _triangles;

        private MeshFilter _meshFilter;

        #region Singleton

        private static MapGenerator Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private IEnumerator Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            
            var mesh = new Mesh();
            
            _vertices = new List<Vector3>
            {
                new(-1.5f, 1.5f),
                new(1.5f, 1.5f),
                new(1.5f, -1.5f),
                new(-1.5f, 1.5f),
            };
            _triangles = new List<int>
            {
                0, 1, 2, 1, 3, 2
            };

            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();

            _meshFilter.mesh = mesh;

            yield return new WaitUntil(() => PlayerManager.Instance != null);

            _structures = Resources.LoadAll<SoStructure>("Structures").ToList();
            
            Generate();
        }

        public void Generate()
        {
            GenerateStructures();
        }

        private void GenerateStructures()
        {
            // TODO: Structure generation 

            // THIS IS DEBUG ONLY
            var structurePrefab = GameManager.Instance.GetPrefab<StructureBase>(PrefabNames.StructureBase);
            for (int i = 0; i < 5; i++)
            {
                foreach (var structure in _structures)
                {
                    var randomOffsetX = Random.Range(-3f, 3f);
                    var randomOffsetY = Random.Range(-3f, 3f);
                    var newPos = transform.position;
                    newPos.x = randomOffsetX;
                    newPos.y = randomOffsetY;
                    Instantiate(structurePrefab, newPos, Quaternion.identity).Setup(structure);
                }
            }
        }

        private void Update()
        {
            
        }

        public static Vector2 GetRandomPos()
        {
            return Vector2.zero;
        }

        public static bool ContainsEntity(Vector2 position)
        {
            return false;
        }
    }
}