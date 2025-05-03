using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new AdditionalDashStackEnchantment", menuName = "Custom/Enchantments/DashStack")]
    public class SoAdditionalDashStackEnchantment : SoEnchantment
    {
        public override string GetDescription()
        {
            return Description;
        }

        public override void OnApply(EnchantmentLogic enchantmentLogic)
        {
            base.OnApply(enchantmentLogic);
            PlayerManager.PlayerMovement.AddDashStack();
        }

        public override void OnRemove(EnchantmentLogic enchantmentLogic)
        {
            base.OnApply(enchantmentLogic);
            PlayerManager.PlayerMovement.AddDashStack(-1);
        }
    }
}