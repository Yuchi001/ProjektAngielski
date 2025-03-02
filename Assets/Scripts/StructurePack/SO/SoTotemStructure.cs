using System;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.InteractionUI;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using Object = UnityEngine.Object;

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

        private string StructureUIKey(StructureBase structureBase) => $"{StructureName}{structureBase.GetInstanceID()}";

        public override void OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            var opened = structureBase.Toggle;
            Time.timeScale = opened ? 0 : 1;
            PlayerManager.Instance.PlayerItemManager.ToggleEq(opened);
            
            if (!opened) UIManager.CloseUI(StructureUIKey(structureBase));
            else
            {
                var totemUIInstance = UIManager.OpenUI<TotemInteractionUI>(StructureUIKey(structureBase), openStrategy, closeStrategy);
                if (!structureBase.WasUsed) totemUIInstance.Setup(this, structureBase);
            }
        }
    }
}