using System.Collections.Generic;
using System.Linq;
using Managers;
using MapGeneratorPack.Enums;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using WorldGenerationPack;

namespace MapPack
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MinMax missionCount;

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

            var missionStructure = Resources.Load<SoMissionStructure>("Structures");
            foreach (var mission in _missions)
            {
                var missionBase = StructureManager.SpawnStructure(missionStructure, mission.GetPos(), transform);
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
            private readonly int _soulCount;
            private readonly int _soulToCoinRatio;
            private readonly int _coinReward;
            private readonly Vector2 _positionOnMap;
            private readonly ERegionType _regionType;
            private readonly EDifficulty _difficulty;

            public Vector2 GetPos() => _positionOnMap;
            public int GetSoulCount() => _soulCount;
            public int GetSoulToCoinRatio() => _soulToCoinRatio;
            public int GetCoinReward() => _coinReward;
            public ERegionType GetRegionType() => _regionType;
            public EDifficulty GetDifficulty() => _difficulty;

            public MissionData(EDifficulty difficulty, Region region)
            {
                _difficulty = difficulty;
                _soulCount = region.RegionData.RandomSoulCount(difficulty);
                _soulToCoinRatio = region.RegionData.RandomSoulPerCoinRatio(difficulty);
                _positionOnMap = region.GetRandomRegionPoint();
                _regionType = region.RegionData.RegionType;
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
                return _difficulty.ToString();
            }
        }
    }
}