using Managers;
using PlayerPack.SO;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    public class SOCharacterStructure : SoStructure
    {
        [SerializeField] private SoCharacter character;
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            MissionManager.PickCharacter(character);
            return true;
        }
    }
}