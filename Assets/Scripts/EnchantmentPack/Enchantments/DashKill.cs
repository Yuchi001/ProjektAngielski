using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class DashKill : EnchantmentBase
    {
        public const float healthPercentage = 0.2f;

        public override string GetDescriptionText()
        {
            return $"When dashing thru enemies kill those with less than {(int)(healthPercentage * 100)}% health.";
        }
    }
}