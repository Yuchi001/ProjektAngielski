using System;
using EnchantmentPack.EnchantmentUIStrategy;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack.EnchantmentLogic
{
    public abstract class StackEnchantmentLogic : EnchantmentLogicBase
    {
        protected SoStackEnchantment _stackEnchantmentData;

        protected Action<int> _onStacksChange;
        
        public override void ApplyEnchantment(SoEnchantment data)
        {
            base.ApplyEnchantment(data);
            _stackEnchantmentData = (SoStackEnchantment)data;
        }

        public void Subscribe(Action<int> onStacksChange)
        {
            _onStacksChange = onStacksChange;
        }
        
        public override void AttachDisplayStrategy(EnchantmentItemAccessor accessor) =>
            accessor.gameObject.AddComponent<StackDisplayStrategy>().SetLogic(this, accessor);
    }
}