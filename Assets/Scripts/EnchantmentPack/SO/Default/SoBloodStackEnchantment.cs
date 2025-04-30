using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new BloodStackEnchantment", menuName = "Custom/Enchantments/BloodStack")]
    public class SoBloodStackEnchantment : SoStackEnchantment
    {
        [SerializeField] private int deathsPerDamage;

        public int DeathsPerDamage => deathsPerDamage;
        
        public override string GetDescription()
        {
            return Description.Replace("$x$", deathsPerDamage.ToString());
        }
    }
}