using System.Collections.Generic;
using EnemyPack.SO;
using MapGeneratorPack.Enums;
using StructurePack.SO;
using UnityEngine;

namespace StagePack
{
    [CreateAssetMenu(fileName = "new Stage Data", menuName = "Custom/Stage")]
    public class SoStage : ScriptableObject
    {
        [SerializeField] private EStageType stageType;
        [SerializeField] private List<SoEnemy> availableEnemies;
        [SerializeField] private List<SoStructure> uniqueStructures;
        [SerializeField] private List<SoStageEffect> uniqueEffects;
        //TODO: TileSheet

        public EStageType StageType => stageType;
        public IEnumerable<SoEnemy> AvailableEnemies => availableEnemies;
        public IEnumerable<SoStructure> UniqueStructures => uniqueStructures;
        public IEnumerable<SoStageEffect> UniqueEffects => uniqueEffects;
    }
}