using EnchantmentPack.EnchantmentLogic;
using UnityEngine;

namespace EnchantmentPack.SO
{
    public abstract class SoStackEnchantment : SoEnchantment
    {
        [SerializeField] private StackEnchantmentLogic enchantmentLogic; 
        
        public override EnchantmentLogicBase GetLogicPrefab()
        {
            return enchantmentLogic;
        }
    }
}