using System.Collections.Generic;
using EnchantmentPack.Enums;
using UnityEngine;

namespace EnchantmentPack
{
    public abstract class EnchantmentBase : MonoBehaviour
    {
        protected SoEnchantment Enchantment { get; private set; }
        protected Dictionary<EValueKey, float> parameters = new();

        public void Setup(SoEnchantment enchantment)
        {
            Enchantment = enchantment;
            foreach (var param in enchantment.EnchantmentParams)
            {
                parameters.Add(param.Key, param.Value);
            }
        }

        public bool Is(SoEnchantment enchantment)
        {
            return Enchantment.EnchantmentName == enchantment.EnchantmentName;
        }

        public SoEnchantment Get()
        {
            return Enchantment;
        }
    }
}