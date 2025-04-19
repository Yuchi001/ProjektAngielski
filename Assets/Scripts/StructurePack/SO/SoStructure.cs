using Managers;
using MinimapPack;
using MinimapPack.Strategies;
using StructurePack.Enum;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using WorldGenerationPack;

namespace StructurePack.SO
{
    public abstract class SoStructure : ScriptableObject
    {
        [SerializeField] protected string structureName;
        [SerializeField, TextArea(3, 10)] protected string structureDescription;
        [SerializeField] protected Sprite structureSprite;
        [SerializeField] protected Sprite activeSprite;
        [SerializeField] protected Sprite usedStructureSprite;
        [SerializeField] protected Sprite minimapIcon;
        [SerializeField] protected float structureScale = 1;
        [SerializeField] protected int interactionLimit;
        [SerializeField] protected string bottomHoverMessage = "Press E";
        [SerializeField] protected string interactionDeclineMessage = "";
        [SerializeField] protected bool usesUI = false;
        [SerializeField] protected bool visibleOnMinimap = false;
        [SerializeField] protected string uIPrefabName = "";

        public float StructureScale => structureScale;
        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public bool Reusable => interactionLimit > 1;
        public int InteractionLimit => interactionLimit;
        public string InteractionDeclineMessage => interactionDeclineMessage;

        public virtual void OnSetup(StructureBase structureBase)
        {
            if (!visibleOnMinimap) return;

            var renderStrategy = new IconRenderStrategy(minimapIcon, structureBase.transform.position);
            WorldGeneratorManager.MinimapManager.RenderOnMinimap($"VISION{structureBase.GetInstanceID()}", renderStrategy);
        }

        public virtual void OnDataChange<T>(T data) where T: class, new()
        {
            
        }

        public virtual IRenderStrategy GetMinimapRenderStrategy(StructureBase structureBase)
        {
            if (!visibleOnMinimap) return null;

            return new IconRenderStrategy(structureSprite, structureBase.transform.position);
        }

        public virtual IOpenStrategy GetOpenStrategy(StructureBase structureBase)
        {
            if (!usesUI) return null;

            var uiPrefab = GameManager.GetPrefab<IStructure>(uIPrefabName);
            uiPrefab.Setup(this, structureBase);
            return new CloseAllOfTypeOpenStrategy<IStructure>((UIBase)uiPrefab, false);
        }

        public virtual ICloseStrategy GetCloseStrategy()
        {
            return !usesUI ? null : new DefaultCloseStrategy();
        }

        public abstract bool OnInteract(StructureBase structureBase);
        
        public virtual void OnFocusChanged(StructureBase structureBase, bool newValue) {}

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
            USED
        }
    }
}