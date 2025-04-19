using System.Globalization;
using EnchantmentPack.EnchantmentLogic;
using EnchantmentPack.EnchantmentLogic.Default;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new AdditionalDashStackEnchantment", menuName = "Custom/Enchantments/BetterHeal")]
    public class SoBetterHealEnchantment : SoEnchantment
    {
        [SerializeField] private float healPercentage;
        [SerializeField] private BetterHealEnchantmentLogic logicPrefab;

        public float HealPercentage => healPercentage;
        
        public override string GetDescription()
        {
            var value = (int)(healPercentage * 100);
            return Description.Replace("$x$", $"{value}%");
        }

        public override EnchantmentLogicBase GetLogicPrefab()
        {
            return logicPrefab;
        }
    }
}