using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission", menuName = "Custom/Structure/Mission")]
    public class SoMissionStructure : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            return true;
        }
    }
}