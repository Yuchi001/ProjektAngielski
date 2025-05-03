using EnemyPack;
using EnemyPack.SO;
using UnityEngine;
using WorldGenerationPack;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mimic Structure", menuName = "Custom/Structure/Mimic")]
    public class SoMimicStructure : SoStructure
    {
        [SerializeField] private SoEnemy mimicEnemy;
        public override bool OnInteract(StructureBase structureBase)
        { 
            EnemyManager.SpawnEnemy(mimicEnemy, structureBase.transform.position);
            Destroy(structureBase.gameObject);
            return true;
        }
    }
}