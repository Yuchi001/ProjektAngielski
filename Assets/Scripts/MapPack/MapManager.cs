using System.Collections.Generic;
using System.Linq;
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

            var shopStructure = Resources.Load<SoShop>("Structures/Shop");
            StructureManager.SpawnStructure(shopStructure, shopPosition.position, transform);

            var missionStructure = Resources.Load<SoMissionStructure>("Structures/MissionStructure");
            foreach (var mission in _missions)
            {
                var missionBase = StructureManager.SpawnStructure(missionStructure, mission.MapPosition, transform);
                missionBase.SetData(mission);
            }
        }

        private MissionData GenerateMission(MissionData.EDifficulty? currentDifficulty = null)
        {
            MissionData.EDifficulty? harderDiff = currentDifficulty != null
                ? (MissionData.EDifficulty)Mathf.Clamp((int)currentDifficulty + 1, 0, System.Enum.GetValues(typeof(MissionData.EDifficulty)).Length)
                : null;
            var region = _regions.RandomElement();
            return new MissionData(harderDiff ?? MissionData.EDifficulty.EASY, region);
        }

        public class MissionData
        {
            private readonly Region _region;

            public Vector2 MapPosition { get; }
            public int SoulCount { get; }
            public int SoulToCoinRatio { get; }
            public int CoinReward { get; }
            public Vector2Int WorldSize { get; }
            public ERegionType RegionType { get; }
            public EDifficulty Difficulty { get; }
            public EThemeType ThemeType { get; }
            public Sprite BackgroundSprite { get; }

            public float GetGapTime() => _region.RegionData.GapGapTime();
            public float GetSpawnRate(float time) => _region.RegionData.GetSpawnRate(time);
            public bool ShouldCreateGap(float time) => _region.RegionData.ShouldCreateGap(time);
            public TileBase GetTile(int x, int y) => _region.RegionData.GetTile(x, y, WorldSize);
            

            public MissionData(EDifficulty difficulty, Region region)
            {
                Difficulty = difficulty;
                SoulCount = region.RegionData.RandomSoulCount(difficulty);
                SoulToCoinRatio = region.RegionData.RandomSoulPerCoinRatio(difficulty);
                MapPosition = region.GetRandomRegionPoint();
                RegionType = region.RegionData.RegionType;
                CoinReward = region.RegionData.RandomReward(difficulty);
                WorldSize = region.RegionData.GetWorldSize();
                ThemeType = region.RegionData.RegionTheme;
                BackgroundSprite = region.RegionData.BackgroundSprite;
                _region = region;
            }
            
            public MissionData(){}

            public enum EDifficulty
            {
                EASY = 0,
                MEDIUM = 1,
                HARD = 2,
                EXPERT = 3
            }

            public string GetDifficultyStr()
            {
                //TODO: translate enum to string
                return Difficulty.ToString();
            }
        }
    }
}