using System;
using System.Collections.Generic;
using EnchantmentPack.Enums;
using PlayerPack;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    public class DisplayEnchantmentUI : MonoBehaviour
    {
        [SerializeField] private GameObject displayEnchantmentSlotPrefab;
        [SerializeField] private RectTransform slotContainer;

        private Dictionary<EEnchantmentName, DisplayEnchantmentSlotUI> _enchantments = new();

        private int? queuedSlotIndex = null;
        
        private void Awake()
        {
            PlayerEnchantmentManager.OnAddEnchantment += AddEnchantment;
            PlayerEnchantmentManager.OnRemoveEnchantment += RemoveEnchantment;
        }

        private void OnDisable()
        {
            PlayerEnchantmentManager.OnAddEnchantment -= AddEnchantment;
            PlayerEnchantmentManager.OnRemoveEnchantment -= RemoveEnchantment;
        }

        private void RemoveEnchantment(EEnchantmentName enchantmentName)
        {
            foreach (var child in slotContainer.GetComponentsInChildren<DisplayEnchantmentSlotUI>())
            {
                if (child.Enchantment.EnchantmentName != enchantmentName) continue;

                queuedSlotIndex = child.GetComponentIndex();
            }
            
            Destroy(_enchantments[enchantmentName]);
            _enchantments.Remove(enchantmentName);
        }

        private void AddEnchantment(SoEnchantment enchantment, EnchantmentBase logic)
        {
            var slotObj = Instantiate(displayEnchantmentSlotPrefab, slotContainer, false);
            var slotScript = slotObj.GetComponent<DisplayEnchantmentSlotUI>();
            slotScript.Setup(enchantment, logic);

            if (queuedSlotIndex.HasValue)
            {
                slotObj.transform.SetSiblingIndex(queuedSlotIndex.Value);
                queuedSlotIndex = null;
            }

            _enchantments.Add(enchantment.EnchantmentName, slotScript);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
        }
    }
}