using UIPack.CloseStrategies;
using UIPack.OpenStrategies;

namespace StructurePack.SO
{
    public class SoMissionBoardStructure : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            return true;
        }
    }
}