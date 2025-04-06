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
                    Instance.tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
            ZoneGeneratorManager.GenerateBaseZone();
        }
    }
}