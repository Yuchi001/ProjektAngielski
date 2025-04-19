using EnchantmentPack.SO;
using EnchantmentPack.SO.Default;
using PlayerPack;
using PlayerPack.Decorators;

namespace EnchantmentPack.EnchantmentLogic.Default
{
    public class BetterHealEnchantmentLogic : EnchantmentLogicBase
    {
        private const string HEAL_MODIFIER_KEY = "EnchantmentHealModifier";
        
        public override void ApplyEnchantment(SoEnchantment data)
        {
            base.ApplyEnchantment(data);
            var healData = (SoBetterHealEnchantment)data;
            PlayerManager.GetHealContextManager().AddHealModifier(HEAL_MODIFIER_KEY, new PercentageHealModifier(healData.HealPercentage));
        }

        public override void RemoveEnchantment()
        {
            PlayerManager.GetHealContextManager().RemoveHealModifier(HEAL_MODIFIER_KEY);
            base.RemoveEnchantment();
        }
    }
}