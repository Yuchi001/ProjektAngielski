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
        
        private void Awake()
        {
            PlayerEnchantmentManager.OnAddEnchantment += AddEnchantment;
        }

        private void OnDisable()
        {
            PlayerEnchantmentManager.OnAddEnchantment -= AddEnchantment;
        }

        private void AddEnchantment(SoEnchantment enchantment, EnchantmentBase logic)
        {
            var slotObj = Instantiate(displayEnchantmentSlotPrefab, slotContainer, false);
            var slotScript = slotObj.GetComponent<DisplayEnchantmentSlotUI>();
            slotScript.Setup(enchantment, logic);

            _enchantments.Add(enchantment.EnchantmentName, slotScript);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
        }
    }
}