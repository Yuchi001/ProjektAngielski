using System.Collections.Generic;
using EnchantmentPack.Enums;
using UnityEngine;

namespace EnchantmentPack
{
    public abstract class EnchantmentBase : MonoBehaviour
    {
        protected readonly Dictionary<EValueKey, float> parameters = new();

        private SoEnchantment _enchantment;
        public SoEnchantment Enchantment => _enchantment; 

        public void Setup(SoEnchantment enchantment)
        {
            _enchantment = enchantment;
            foreach (var param in enchantment.EnchantmentParams)
            {
                parameters.Add(param.Key, param.Value);
            }
        }
    }
}