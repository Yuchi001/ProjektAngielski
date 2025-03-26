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
        [SerializeField] private Sprite usedStructureSprite;
        [SerializeField] private float structureScale = 1;
        [SerializeField] private bool reusable = true;
        [SerializeField] private bool maintainData = true;
        [SerializeField] private string bottomHoverMessage = "Press E";
        [SerializeField] private bool usesUI = false;
        [SerializeField] private string uIPrefabName = "";

        public float StructureScale => structureScale;
        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public Sprite StructureSprite => structureSprite;
        public Sprite UsedStructureSprite => usedStructureSprite;
        public bool Reusable => reusable;
        public bool MaintainData => maintainData;
        public bool UsesUI => usesUI;
        public string UIPrefabName => uIPrefabName;

        public abstract void OnInteract(StructureBase structureBase, 
            IOpenStrategy openStrategy,
            ICloseStrategy closeStrategy);

        public virtual string GetInteractionMessage(StructureBase structureBase)
        {
            return bottomHoverMessage;
        }
    }
}