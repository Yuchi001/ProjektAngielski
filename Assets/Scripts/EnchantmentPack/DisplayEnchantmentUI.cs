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
            PlayerEnchantmentManager.OnAddEnchantment += AddEnchantment;
        }

        private void OnDisable()
        {
            PlayerEnchantmentManager.OnAddEnchantment -= AddEnchantment;
        }

        private void AddEnchantment(SoEnchantment enchantment, EnchantmentBase logic)
        {
            var slotObj = Instantiate(displayEnchantmentSlotPrefab, slotContainer, false);
            slotObj.GetComponent<DisplayEnchantmentSlotUI>().Setup(enchantment, logic);

            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
        }
    }
}