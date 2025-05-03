using System;
using EnchantmentPack.SO;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public sealed class StackDisplayStrategy : EnchantmentDisplayStrategy
    {
        private int _currentStacks = 0;
        private SoStackEnchantment.StackData _data;
        private EnchantmentItemAccessor _accessor;
        
        public override void SetDisplayData(EnchantmentLogic logicBase, EnchantmentItemAccessor uiAccessor)
        {
            base.SetDisplayData(logicBase, uiAccessor);

            _data = logicBase.GetData<SoStackEnchantment.StackData>();
            _accessor = uiAccessor;
            
            uiAccessor.CornerText.gameObject.SetActive(true);
            uiAccessor.CornerText.text = "0";
            _currentStacks = 0;
        }

        private void Update()
        {
            var newStackCount = _data.StackCount;
            if (_currentStacks == newStackCount) return;

            _currentStacks = newStackCount;
            _accessor.CornerText.text = newStackCount.ToString();
        }
    }
}