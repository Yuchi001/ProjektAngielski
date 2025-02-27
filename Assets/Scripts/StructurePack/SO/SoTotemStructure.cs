using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.InteractionUI;
using UIPack;
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
            Time.timeScale = 0;
            PlayerManager.Instance.PlayerItemManager.ToggleEq(true);
            if (spawnedStructureInteraction != null)
            {
                spawnedStructureInteraction.SetActive(true);
                return;
            }
            
            var prefab = GameManager.Instance.GetPrefab<TotemInteractionUI>(PrefabNames.TotemInteractionUI);
            
            //TODO: zunifikuj z uiManagerem
            /*var spawned = Instantiate(prefab, UIManager.Instance.GameCanvas);
            spawned.Setup(this);
            spawnedStructureInteraction = spawned.gameObject;*/
        }
    }
}