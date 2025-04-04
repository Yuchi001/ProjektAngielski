using AudioPack;
using Managers;
using Managers.Enums;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Door", menuName = "Custom/Structure/Door")]
    public class SoDoorStructure : SoStructure
    {
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            GameManager.OpenMap();
            Destroy(structureBase.gameObject);
            AudioManager.PlaySound(ESoundType.Chest); // TODO: [SOUND] door sound
            return true;
        }
    }
}