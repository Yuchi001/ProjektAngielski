using EnchantmentPack.EnchantmentLogic;
using EnchantmentPack.EnchantmentLogic.Default;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new AdditionalDashStackEnchantment", menuName = "Custom/Enchantments/DashStack")]
    public class SoAdditionalDashStackEnchantment : SoEnchantment
    {
        [SerializeField] private AdditionalDashStackEnchantmentLogic logicPrefab;
        
        public override string GetDescription()
        {
            return Description;
        }

        public override EnchantmentLogicBase GetLogicPrefab()
        {
            return logicPrefab;
        }
    }
}