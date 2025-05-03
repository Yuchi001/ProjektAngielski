using EnchantmentPack.EnchantmentUIStrategy;
using UnityEngine;

namespace EnchantmentPack.SO
{
    public abstract class SoStackEnchantment : SoEnchantment
    {
        public override void OnApply(EnchantmentLogic enchantmentLogic)
        {
            base.OnApply(enchantmentLogic);
            enchantmentLogic.SetData(new StackData());
        }

        public override void HandleDisplayStrategy(EnchantmentItemAccessor accessor, EnchantmentLogic logic)
        {
            var displayStrategy = accessor.gameObject.AddComponent<StackDisplayStrategy>();
            displayStrategy.SetDisplayData(logic, accessor);
        }

        public class StackData : EnchantmentLogic.Data
        {
            public int StackCount { get; private set; } = 0;
            public void IncrementStacks() => StackCount++;
            public void SetStackCount(int stackCount) => StackCount = stackCount;
        }
    }
}