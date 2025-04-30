using EnchantmentPack.SO;
using EnchantmentPack.SO.Default;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.EnchantmentLogic.Default
{
    public class PassiveHealEnchantmentLogic : CooldownEnchantmentLogic
    {
        private SoPassiveHealEnchantment _passiveHealData;
        
        public override void ApplyEnchantment(SoEnchantment data)
        {
            base.ApplyEnchantment(data);
            _passiveHealData = (SoPassiveHealEnchantment)data;
        }

        protected override bool TryUseEnchant()
        {
            var maxHealth = PlayerManager.PlayerHealth.MaxHealth;
            if (PlayerManager.PlayerHealth.CurrentHealth >= maxHealth) return false;

            PlayerManager.PlayerHealth.Heal(Mathf.CeilToInt(maxHealth * _passiveHealData.HealthPercentage));
            return true;
        }
    }
}