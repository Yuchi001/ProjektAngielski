using UnityEngine;
using UnityEngine.Serialization;

namespace EnchantmentPack
{
    public abstract class EnchantmentBase : MonoBehaviour
    {
        private SoEnchantment _enchantment;

        public void Setup(SoEnchantment enchantment)
        {
            _enchantment = enchantment;
        }

        public bool Is(SoEnchantment enchantment)
        {
            return _enchantment.EnchantmentName == enchantment.EnchantmentName;
        }

        public SoEnchantment Get()
        {
            return _enchantment;
        }

        public abstract string GetDescriptionText();
    }
}