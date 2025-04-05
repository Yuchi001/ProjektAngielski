using MapPack;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission", menuName = "Custom/Structure/Mission")]
    public class SoMissionStructure : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase)
        {
            return true;
        }

        public override void OnDataChange<T>(T data) where T: class
        {
            var validData = data as MapManager.MissionData;
        }
    }
}