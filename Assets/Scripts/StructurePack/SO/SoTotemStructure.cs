using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.InteractionUI;
using UnityEngine;

namespace StructurePack.SO
{
    public class SoTotemStructure : SoStructure
    {
        [SerializeField] private int enchantmentCount;
        [SerializeField] private float costModifierPerPurchase;
        [SerializeField] private int rechargeIncrementCost;
        [SerializeField] private int baseRechargeCost;

        public int EnchantmentCount => enchantmentCount;
        public float CostModifierPerPurchase => costModifierPerPurchase;
        public int RechargeIncrementCost => rechargeIncrementCost;
        public int BaseRechargeCost => baseRechargeCost;
        
        public override void OnInteract(StructureBase structureBase, ref GameObject spawnedStructureInteraction)
        {
            PlayerManager.Instance.PlayerItemManager.ToggleEq(true);
            if (spawnedStructureInteraction != null)
            {
                spawnedStructureInteraction.SetActive(true);
                return;
            }
            
            var prefab = GameManager.Instance.GetPrefab<TotemInteractionUI>(PrefabNames.TotemInteractionUI);
            var spawned = Instantiate(prefab, GameUiManager.Instance.GameCanvas);
            spawned.Setup(this);
            spawnedStructureInteraction = spawned.gameObject;
        }
    }
}