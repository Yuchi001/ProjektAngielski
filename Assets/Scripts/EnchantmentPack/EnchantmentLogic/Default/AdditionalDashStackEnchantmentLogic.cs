using EnchantmentPack.SO;
using PlayerPack;

namespace EnchantmentPack.EnchantmentLogic.Default
{
    public class AdditionalDashStackEnchantmentLogic : EnchantmentLogicBase
    {
        public override void ApplyEnchantment(SoEnchantment data)
        {
            base.ApplyEnchantment(data);
            PlayerManager.PlayerMovement.AddDashStack();
        }
    }
}