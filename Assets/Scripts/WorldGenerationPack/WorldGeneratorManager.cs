using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using GameLoaderPack;
using Managers;
using Managers.Other;
using MapPack;
using MinimapPack;
using MinimapPack.Strategies;
using PlayerPack;
using StructurePack;
using StructurePack.SO;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

namespace WorldGenerationPack
{
    public class WorldGeneratorManager : MonoBehaviour, IMissionDependentInstance
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private MinMax visionDistance;
        [SerializeField] private float visionPerSquare;
        
        private static WorldGeneratorManager Instance { get; set; }
        public static MinimapManager MinimapManager { get; private set; }
        public static EnemySpawner EnemySpawner { get; private set; }
        private const string MINIMAP_UI_KEY = "MINIMAP_UI_KEY";

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void OnDestroy()
        {
            if (MinimapManager != null) UIManager.CloseUI(MINIMAP_UI_KEY);
        }

        public void Init(MapManager.MissionData missionData)
        {
            background.sprite = missionData.BackgroundSprite;   
            var worldSize = missionData.WorldSize;
            var offset = new Vector3Int(worldSize.x / 2, worldSize.y / 2, 0);

            var minimapPrefab = GameManager.GetPrefab<MinimapManager>(PrefabNames.MinimapManager);
            var openMinimapStrategy = new SingletonOpenStrategy<MinimapManager>(minimapPrefab);
            var closeMinimapStrategy = new DestroyCloseStrategy(MINIMAP_UI_KEY);
            MinimapManager = UIManager.OpenUI<MinimapManager>(MINIMAP_UI_KEY, openMinimapStrategy, closeMinimapStrategy);
            MinimapManager.SetupMinimap(worldSize);
            
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
                    var spawnPos = (Vector3)current - offset + new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
                    var structurePair = missionData.Structures.FirstOrDefault(p => p.position == (Vector2Int)current);

                    if (exitPosition == (Vector2Int)current)
                    {
                        var structureBase = StructureManager.SpawnStructure(exitStructure, spawnPos, GameManager.EScene.GAME);
                        exitStructure.SetMission(structureBase, missionData);
                    }
                    else if (visionPlacements.Contains((Vector2Int)current)) placedVisions.Add(StructureManager.SpawnStructure(visionStructure, spawnPos, GameManager.EScene.GAME));
                    else if (structurePair != default) StructureManager.SpawnStructure(structurePair.structure, spawnPos, GameManager.EScene.GAME);
                    
                    Instance.tilemap.SetTile(current - offset, tile);
                }
            }

            var randomVision =  placedVisions.RandomElement();
            visionStructure.SetZone(randomVision);
            PlayerManager.SetPosition(randomVision.transform.position);
            MinimapManager.RenderOnMinimap("MAIN_PLAYER", new FollowRenderStrategy(PlayerManager.GetTransform(), PlayerManager.PickedCharacter.CharacterIcon));

            var spawnerPrefab = missionData.SpawnerPrefab;
            EnemySpawner = Instantiate(spawnerPrefab, transform);
            EnemySpawner.Setup(missionData);
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