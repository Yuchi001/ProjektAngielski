using EnchantmentPack.EnchantmentUIStrategy;
using UnityEngine;

namespace EnchantmentPack.SO
{
    public abstract class SoCooldownEnchantment : SoEnchantment
    {
        [SerializeField] private float cooldown;

        public float Cooldown => cooldown;

        public override void OnApply(EnchantmentLogic enchantmentLogic)
        {
            base.OnApply(enchantmentLogic);
            enchantmentLogic.SetData(new CooldownData((SoCooldownEnchantment)enchantmentLogic.Enchantment));
        }

        public override void OnUpdate(EnchantmentLogic enchantmentLogic)
        {
            base.OnUpdate(enchantmentLogic);
            var data = enchantmentLogic.GetData<CooldownData>();
            data.ModifyCurrent(Time.deltaTime);
            if (data.GetPercentage() < 0.99f) return;

            var success = OnTrigger(enchantmentLogic);
            if (success) data.Reset();
        }

        public override void HandleDisplayStrategy(EnchantmentItemAccessor accessor, EnchantmentLogic logic)
        {
            var displaysStrategy = accessor.gameObject.AddComponent<CdrDisplayStrategy>();
            displaysStrategy.SetDisplayData(logic, accessor);
        }

        public abstract bool OnTrigger(EnchantmentLogic enchantmentLogic);

        public class CooldownData : EnchantmentLogic.Data
        {
            private float _current = 0;
            private readonly float _max;
            
            public CooldownData(SoCooldownEnchantment enchantment)
            {
                _current = 0;
                _max = enchantment.Cooldown;
            }

            public void ModifyCurrent(float value) => _current += value;

            public void Reset() => _current = 0;

            public float GetPercentage() => _current / _max;
        }
    }
}