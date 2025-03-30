using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    public abstract class SoStructure : ScriptableObject
    {
        [SerializeField] private string structureName;
        [SerializeField, TextArea(3, 10)] private string structureDescription;
        [SerializeField] private Sprite structureSprite;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite usedStructureSprite;
        [SerializeField] private float structureScale = 1;
        [SerializeField] private int interactionLimit;
        [SerializeField] private bool maintainData = true;
        [SerializeField] private string bottomHoverMessage = "Press E";
        [SerializeField] private string interactionDeclineMessage = "";
        [SerializeField] private bool usesUI = false;
        [SerializeField] private string uIPrefabName = "";

        public float StructureScale => structureScale;
        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public bool Reusable => interactionLimit > 1;
        public int InteractionLimit => interactionLimit;
        public bool MaintainData => maintainData;
        public bool UsesUI => usesUI;
        public string UIPrefabName => uIPrefabName;
        public string InteractionDeclineMessage => interactionDeclineMessage;

        public abstract bool OnInteract(StructureBase structureBase, 
            IOpenStrategy openStrategy,
            ICloseStrategy closeStrategy);

        public virtual string GetInteractionMessage(StructureBase structureBase)
        {
            return bottomHoverMessage;
        }

        public Sprite GetSprite(EState state) => state switch
        {
            EState.NOT_USED => structureSprite,
            EState.ACTIVE => activeSprite != null ? activeSprite : structureSprite,
            EState.USED => usedStructureSprite != null ? usedStructureSprite : structureSprite,
            _ => structureSprite
        };

        public enum EState
        {
            NOT_USED,
            ACTIVE,
            USED,
        }
    }
}