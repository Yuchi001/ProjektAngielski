using System.Collections.Generic;
using EnemyPack.SO;
using MapPack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using WorldGenerationPack.Enums;

namespace StagePack
{
    [CreateAssetMenu(fileName = "new Region Data", menuName = "Custom/Region")]
    public class SoRegion : ScriptableObject
    {
        [SerializeField] private ERegionType regionType;
        [SerializeField] private List<SoStructure> uniqueStructures;
        [SerializeField] private List<SoStageEffect> uniqueEffects;
        [SerializeField] private MinMax soulToCoinRatioPerDiff;
        [SerializeField] private MinMax soulToCoinRatioBase;
        [SerializeField] private MinMax soulCountPerDiff;
        [SerializeField] private MinMax soulCountBase;
        [SerializeField] private MinMax coinRewardPerDiff;
        [SerializeField] private MinMax coinRewardBase;
        [SerializeField] private AnimationCurve difficultyScalingCurve;
        [SerializeField] private AnimationCurve waveGapChanceCurve;
        [SerializeField] private MinMax gapTime;
        
        //TODO: TileSheet

        public ERegionType RegionType => regionType;
        public IEnumerable<SoStructure> UniqueStructures => uniqueStructures;
        public IEnumerable<SoStageEffect> UniqueEffects => uniqueEffects;
        public int RandomSoulCount(MapManager.MissionData.EDifficulty difficulty)
        {
            var result = soulCountBase.RandomInt();
            var diff = (int)difficulty;
            for (var i = 0; i < diff; i++)
            {
                result += soulCountPerDiff.RandomInt();
            }

            return result;
        }

        public int RandomSoulPerCoinRatio(MapManager.MissionData.EDifficulty difficulty)
        {
            var result = soulToCoinRatioBase.RandomInt();
            var maxDiff = System.Enum.GetValues(typeof(MapManager.MissionData.EDifficulty)).Length;
            var diff = maxDiff - (int)difficulty;
            for (var i = 0; i < diff; i++)
            {
                result += soulToCoinRatioPerDiff.RandomInt();
            }

            return result;
        }

        public int RandomReward(MapManager.MissionData.EDifficulty difficulty)
        {
            var result = coinRewardBase.RandomInt();
            var diff = (int)difficulty;
            for (var i = 0; i < diff; i++)
            {
                result += coinRewardPerDiff.RandomInt();
            }

            return result;
        }

        public bool ShouldCreateGap(float scale) => Random.Range(0f, 1f) < waveGapChanceCurve.Evaluate(scale);

        public float SpawnRate(float scale) => difficultyScalingCurve.Evaluate(scale);

        public float GapTime() => gapTime.RandomFloat();
    }
}