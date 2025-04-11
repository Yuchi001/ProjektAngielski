using System.Collections.Generic;
using System.Linq;
using GameLoaderPack;
using Managers;
using MapPack;
using PlayerPack;
using StructurePack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace WorldGenerationPack
{
    public class WorldGeneratorManager : MonoBehaviour, IMissionDependentInstance
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private MinMax visionDistance;
        [SerializeField] private float visionPerSquare;
        
        private static WorldGeneratorManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }


        public void Init(MapManager.MissionData missionData)
        {
            background.sprite = missionData.BackgroundSprite;   
            var worldSize = missionData.WorldSize;
            var offset = new Vector3Int(worldSize.x / 2, worldSize.y / 2, 0);
            
            var visionStructure = Resources.Load<SoVisionStructure>("Structures/Vision");
            var visionPlacements = PlaceVision(worldSize, missionData.Structures.Select(s => s.position));
            var placedVisions = new List<StructureBase>();

            var exitStructure = Resources.Load<SoMissionExitStructure>("Structures/MissionExitStructure");
            var exitPosition = new Vector2Int(
                Random.Range(3, worldSize.x - 3),
                Random.Range(3, worldSize.y - 3)
            );
            for (var y = 0; y < worldSize.y; y++)
            {
                for (var x = 0; x < worldSize.x; x++)
                {
                    var tile = missionData.GetTile(x, y);
                    var current = new Vector3Int(x, y, 0);
                    var spawnPos = (Vector3)current - offset;
                    var structurePair = missionData.Structures.FirstOrDefault(p => p.position == (Vector2Int)current);

                    if (exitPosition == (Vector2Int)current)
                    {
                        var structureBase = StructureManager.SpawnStructure(exitStructure, exitPosition, GameManager.EScene.GAME);
                        exitStructure.SetMission(structureBase, missionData);
                    }
                    else if (visionPlacements.Contains((Vector2Int)current)) placedVisions.Add(StructureManager.SpawnStructure(visionStructure, spawnPos, GameManager.EScene.GAME));
                    else if (structurePair != default) StructureManager.SpawnStructure(structurePair.structure, spawnPos, GameManager.EScene.GAME);
                    
                    Instance.tilemap.SetTile(current - offset, tile);
                }
            }

            var randomVision = placedVisions.RandomElement();
            visionStructure.SetZone(randomVision);
            PlayerManager.SetPosition(randomVision.transform.position);
        }
        
        private List<Vector2Int> PlaceVision(Vector2Int worldSize,  IEnumerable<Vector2Int> exceptList)
        {
            var visionCount = worldSize.x * worldSize.y * visionPerSquare;
            var placedTotems = new List<Vector2Int>();
            var attempts = 0;

            while (placedTotems.Count < visionCount && attempts < 10000)
            {
                attempts++;

                var candidate = new Vector2Int(
                    Random.Range(3, worldSize.x - 3),
                    Random.Range(3, worldSize.y - 3)
                );

                var tooClose = placedTotems.Any(p => Vector2Int.Distance(p, candidate) < visionDistance.Min);
                var tooFar = placedTotems.All(p => Vector2Int.Distance(p, candidate) > visionDistance.Max);

                if (placedTotems.Count == 0 || (!tooClose && !tooFar && !exceptList.Contains(candidate)))
                {
                    placedTotems.Add(candidate);
                }
            }

            return placedTotems;
        }
    }
}