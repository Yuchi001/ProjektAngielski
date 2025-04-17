using EnchantmentPack.EnchantmentUIStrategy;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack.EnchantmentLogic
{
    public abstract class CooldownEnchantmentLogic : EnchantmentLogicBase
    {
        protected SoCooldownEnchantment _cooldownData;

        private float _timer = 0;

        public float CooldownPercentage => _timer / _cooldownData.Cooldown;

        public override void ApplyEnchantment(SoEnchantment data)
        {
            base.ApplyEnchantment(data);
            _cooldownData = (SoCooldownEnchantment)data;
        }

        protected virtual void Update()
        {
            _timer += Time.deltaTime;
        }

        public override void AttachDisplayStrategy(EnchantmentItemAccessor accessor) =>
            accessor.gameObject.AddComponent<CdrDisplayStrategy>().SetLogic(this, accessor);
    }
}