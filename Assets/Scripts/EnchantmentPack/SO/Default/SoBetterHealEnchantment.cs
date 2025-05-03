using PlayerPack;
using PlayerPack.Decorators;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new AdditionalDashStackEnchantment", menuName = "Custom/Enchantments/BetterHeal")]
    public class SoBetterHealEnchantment : SoEnchantment
    {
        [SerializeField] private float healPercentage;

        private const string MODIFIER_KEY = "better_heal";

        public override string GetDescription()
        {
            var value = (int)(healPercentage * 100);
            return Description.Replace("$x$", $"{value}%");
        }

        public override void OnApply(EnchantmentLogic enchantmentLogic)
        {
            base.OnApply(enchantmentLogic);
            PlayerManager.GetHealContextManager().AddHealModifier(MODIFIER_KEY, new PercentageHealModifier(healPercentage));
        }

        public override void OnRemove(EnchantmentLogic enchantmentLogic)
        {
            base.OnRemove(enchantmentLogic);
            PlayerManager.GetHealContextManager().RemoveHealModifier(MODIFIER_KEY);
        }
    }
}