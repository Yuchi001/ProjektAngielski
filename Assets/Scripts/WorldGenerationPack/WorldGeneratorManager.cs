using System.Linq;
using GameLoaderPack;
using MainCameraPack;
using Managers;
using MapPack;
using StagePack;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace WorldGenerationPack
{
    public class WorldGeneratorManager : MonoBehaviour, IMissionDependentInstance
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private SpriteRenderer background;

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
            for (var y = 0; y < worldSize.y; y++)
            {
                for (var x = 0; x < worldSize.x; x++)
                {
                    var tile = missionData.GetTile(x, y);
                    var current = new Vector3Int(x, y, 0);
                    var structurePair = missionData.Structures.FirstOrDefault(p => p.position == (Vector2Int)current);
                    if (structurePair != default) StructureManager.SpawnStructure(structurePair.structure, (Vector3)current, transform);
                    Instance.tilemap.SetTile(current, tile);
                }
            }
            ZoneGeneratorManager.GenerateBaseZone();
        }
    }
}