using UnityEngine;

namespace EnchantmentPack.Interfaces
{
    public interface ICooldownEnchantment
    {
        public float MaxCooldown { get; }
        public float CurrentTime { get; }
        public bool IsActive { get; }

        public bool Ready()
        {
            return CurrentTime >= 1;
        }
    }
}