using UIPack.CloseStrategies;
using UIPack.OpenStrategies;

namespace StructurePack.SO
{
    
    //TODO: implement shop 
    public class SoShop : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            return false;
        }
    }
}