using System;
using System.Collections.Generic;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.UI;

namespace StructurePack.InteractionUI
{
    public class TotemInteractionUI : MonoBehaviour, IStructure
    {
        [SerializeField] private Transform enchantmentsHolder;

        [SerializeField] private Button rechargeButton;

        private SoTotemStructure _totem;
        private int _rechargeCost;
        
        public void Setup(SoStructure structureData)
        {
            _totem = (SoTotemStructure)structureData;
            _rechargeCost = _totem.BaseRechargeCost;
            var slotPrefab = GameManager.Instance.GetPrefab<TotemSlotUI>(PrefabNames.TotemSlot);
            var randomEnchantments = PlayerManager.Instance.PlayerEnchantments.GetRandomEnchantmentList(_totem.EnchantmentCount);
            foreach (var enchantment in randomEnchantments)
            {
                var slot = Instantiate(slotPrefab, enchantmentsHolder);
                slot.Setup(enchantment);
            }
        }

        public void IncrementRechargeCost()
        {
            _rechargeCost += _totem.RechargeIncrementCost;
        }

        public void Recharge()
        {
            Setup(_totem);
        }

        private void Update()
        {
            rechargeButton.interactable = _rechargeCost <= PlayerManager.Instance.PlayerExp.StackedLevels;
        }
    }
}