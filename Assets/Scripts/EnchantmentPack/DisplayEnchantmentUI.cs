using System;
using PlayerPack;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    public class DisplayEnchantmentUI : MonoBehaviour
    {
        [SerializeField] private GameObject displayEnchantmentSlotPrefab;
        [SerializeField] private RectTransform slotContainer;
        
        private void Awake()
        {
            PlayerEnchantmentManager.OnEnchantmentAdd += AddEnchantment;
        }

        private void OnDisable()
        {
            PlayerEnchantmentManager.OnEnchantmentAdd -= AddEnchantment;
        }

        private void AddEnchantment(EnchantmentBase enchantment)
        {
            var slotObj = Instantiate(displayEnchantmentSlotPrefab, slotContainer, false);
            slotObj.GetComponent<DisplayEnchantmentSlotUI>().Setup(enchantment);

            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
        }
    }
}