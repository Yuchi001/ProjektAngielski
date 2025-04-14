using Managers;
using MapPack;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission", menuName = "Custom/Structure/Mission")]
    public class SoMissionStructure : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase)
        {
            GameManager.StartRun(structureBase.GetData<MapManager.MissionData>());
            return true;
        }
    }
}