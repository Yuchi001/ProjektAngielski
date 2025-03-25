using StructurePack.Totem;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;

namespace StructurePack.SO
{
    public class SoTotemStructure : SoStructure
    {
        public override void OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            // TODO totem interaction!
        }

        public override string GetInteractionMessage(StructureBase structureBase)
        {
            var data = structureBase.GetData<TotemData>();
            return base.GetInteractionMessage(structureBase).Replace("$soulCount$", GetCurrentSoulPrice(data).ToString());
        }

        public int GetCurrentSoulPrice(TotemData totemData)
        {
            return 0;
        }
    }
}