using System.Collections.Generic;
using EnemyPack.SO;
using MapGeneratorPack.Enums;
using MapPack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace StagePack
{
    [CreateAssetMenu(fileName = "new Stage Data", menuName = "Custom/Stage")]
    public class SoRegion : ScriptableObject
    {
        [FormerlySerializedAs("stageType")] [SerializeField] private ERegionType regionType;
        [SerializeField] private List<SoStructure> uniqueStructures;
        [SerializeField] private List<SoStageEffect> uniqueEffects;
        [SerializeField] private MinMax soulToCoinRatioPerDiff;
        [SerializeField] private MinMax soulToCoinRatioBase;
        [SerializeField] private MinMax soulCountPerDiff;
        [SerializeField] private MinMax soulCountBase;
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
            var diff = (int)difficulty;
            for (var i = 0; i < diff; i++)
            {
                result += soulToCoinRatioPerDiff.RandomInt();
            }

            return result;
        }
    }
}