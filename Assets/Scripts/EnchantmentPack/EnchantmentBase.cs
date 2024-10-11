using UnityEngine;

namespace EnchantmentPack
{
    public class EnchantmentBase : MonoBehaviour
    {
        [SerializeField] private SoEnchantment _enchantment;

        public bool Is(SoEnchantment enchantment)
        {
            return _enchantment.EnchantmentName == enchantment.EnchantmentName;
        }
    }
}