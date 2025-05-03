using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new PassiveHealEnchantment", menuName = "Custom/Enchantments/PassiveHeal")]
    public class SoPassiveHealEnchantment : SoCooldownEnchantment
    {
        [SerializeField] private float healthPercentage;

        public float HealthPercentage => healthPercentage;
        
        public override string GetDescription()
        {
            return Description.Replace("$x$", $"{Mathf.CeilToInt(healthPercentage * 100f)}%").Replace("$y$", Cooldown.ToString());
        }

        public override bool OnTrigger(EnchantmentLogic enchantmentLogic)
        {
            var maxHealth = PlayerManager.PlayerHealth.MaxHealth;
            if (PlayerManager.PlayerHealth.CurrentHealth >= maxHealth) return false;

            PlayerManager.PlayerHealth.Heal(Mathf.CeilToInt(maxHealth * healthPercentage));
            return true;
        }
    }
}