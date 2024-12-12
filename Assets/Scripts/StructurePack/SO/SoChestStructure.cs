using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Chest", menuName = "Custom/Structure/Chest")]
    public class SoChestStructure : SoStructure
    {
        public override void OnInteract(StructureBase structureBase)
        {
            Debug.Log("interact");
        }
    }
}