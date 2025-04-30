using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers;
using Managers.Enums;
using StagePack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;
using WorldGenerationPack;
using WorldGenerationPack.Enums;

namespace MapPack
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MinMax missionCount;
        [SerializeField] private Transform shopPosition;

        private List<Region> _regions;
        private List<MissionData> _missions;

        private void Awake()
        {
            if (!GameManager.HasInstance())
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                GameManager.LoadMenu();
                Destroy(gameObject);
                return;
            }
            
            _regions = FindObjectsOfType<Region>().ToList();
            _missions = GameManager.GetMissions();
            while (_missions.Count < missionCount.RandomInt())
            {
                _missions.Add(GenerateMission());
            }

            if (GameManager.StageCount > 1)
            {
                var shopStructure = Resources.Load<SoShop>("Structures/Shop");
                StructureManager.SpawnStructure(shopStructure, shopPosition.position, GameManager.EScene.MAP);
            }

            var missionStructure = Resources.Load<SoMissionStructure>("Structures/MissionStructure");
            foreach (var mission in _missions)
            {
                var missionBase = StructureManager.SpawnStructure(missionStructure, mission.MapPosition, GameManager.EScene.MAP);
                var structureData = SoMissionStructure.CreateMissionStructureData(mission);
                missionBase.SetData(structureData);
            }
            
            GameManager.SetMissions(_missions);
        }

        private MissionData GenerateMission(DifficultyExtensions.EDifficulty? currentDifficulty = null)
        {
            DifficultyExtensions.EDifficulty? harderDiff = currentDifficulty != null
                ? (DifficultyExtensions.EDifficulty)Mathf.Clamp((int)currentDifficulty + 1, 0, System.Enum.GetValues(typeof(DifficultyExtensions.EDifficulty)).Length)
                : null;
            var region = _regions.RandomElement();
            return new MissionData(harderDiff ?? DifficultyExtensions.EDifficulty.EASY, region);
        }

        public class MissionData
        {
            private readonly Region _region;

            public Vector2 MapPosition { get; }
            public int SoulCount { get; }
            public int CoinReward { get; }
            public Vector2Int WorldSize { get; }
            public ERegionType RegionType { get; }
            public DifficultyExtensions.EDifficulty Difficulty { get; }
            public EThemeType ThemeType { get; }
            public Sprite BackgroundSprite { get; }
            public EnemySpawner SpawnerPrefab { get; }
            public float GetScaledDifficulty(float time) => _region.RegionData.GetScaledDifficulty(time, Difficulty);
            public TileBase GetTile(int x, int y) => _region.RegionData.GetTile(x, y, WorldSize);
            public List<(Vector2Int position, SoStructure structure)> Structures { get; }
            public MissionData(DifficultyExtensions.EDifficulty difficulty, Region region)
            {
                Difficulty = difficulty;
                SoulCount = region.RegionData.RandomSoulCount(difficulty);
                MapPosition = region.GetRandomRegionPoint();
                RegionType = region.RegionData.RegionType;
                CoinReward = region.RegionData.RandomReward(difficulty);
                WorldSize = region.RegionData.GetWorldSize();
                ThemeType = region.RegionData.RegionTheme;
                BackgroundSprite = region.RegionData.BackgroundSprite;
                Structures = region.RegionData.GetStructures(WorldSize);
                SpawnerPrefab = region.RegionData.EnemySpawner;
                _region = region;
            }
            
            public MissionData(){}
        }
    }
}