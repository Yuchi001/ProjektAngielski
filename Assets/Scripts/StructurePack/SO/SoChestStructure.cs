using Managers;
using Managers.Other;
using StructurePack.InteractionUI;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Chest", menuName = "Custom/Structure/Chest")]
    public class SoChestStructure : SoStructure
    {
        [SerializeField, Range(0, 1)] private float randomPercentageBust = 0;
        [SerializeField, Min(1)] private int maxItems = 0;
        
        public float RandomPercentageBust => randomPercentageBust;
        public int MaxItems => maxItems;
        
        public override void OnInteract(StructureBase structureBase)
        {
            var interactionUI = GameManager.Instance.GetPrefab<ChestInteractionUI>(PrefabNames.ChestInteractionUI);
            Instantiate(interactionUI, GameUiManager.Instance.GameCanvas).Setup(this);
        }
    }
}