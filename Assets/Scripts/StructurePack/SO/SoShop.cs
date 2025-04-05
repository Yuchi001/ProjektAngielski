using UnityEngine;

namespace StructurePack.SO
{
    
    //TODO: implement shop 
    [CreateAssetMenu(fileName = "new Shop", menuName = "Custom/Structure/Shop")]
    public class SoShop : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase)
        {
            return false;
        }
    }
}