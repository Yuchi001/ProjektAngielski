using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    public abstract class SoStructure : ScriptableObject
    {
        [SerializeField] protected string structureName;
        [SerializeField, TextArea(3, 10)] protected string structureDescription;
        [SerializeField] protected Sprite structureSprite;
        [SerializeField] protected Sprite activeSprite;
        [SerializeField] protected Sprite usedStructureSprite;
        [SerializeField] protected float structureScale = 1;
        [SerializeField] protected int interactionLimit;
        [SerializeField] protected bool maintainData = true;
        [SerializeField] protected string bottomHoverMessage = "Press E";
        [SerializeField] protected string interactionDeclineMessage = "";
        [SerializeField] protected bool usesUI = false;
        [SerializeField] protected string uIPrefabName = "";

        public float StructureScale => structureScale;
        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public bool Reusable => interactionLimit > 1;
        public int InteractionLimit => interactionLimit;
        public bool MaintainData => maintainData;
        public bool UsesUI => usesUI;
        public string UIPrefabName => uIPrefabName;
        public string InteractionDeclineMessage => interactionDeclineMessage;

        public virtual void OnSetup(StructureBase structureBase)
        {
            
        }

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