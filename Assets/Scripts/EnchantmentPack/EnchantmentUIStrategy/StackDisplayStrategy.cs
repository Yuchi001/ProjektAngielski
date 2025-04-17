using EnchantmentPack.EnchantmentLogic;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public sealed class StackDisplayStrategy : EnchantmentDisplayStrategy
    {
        private EnchantmentItemAccessor _accessor;
        
        public override void SetLogic(EnchantmentLogicBase logicBase, EnchantmentItemAccessor uiAccessor)
        {
            base.SetLogic(logicBase, uiAccessor);

            var stackEnchantmentLogic = (StackEnchantmentLogic)logicBase;
            _accessor = uiAccessor;
            
            uiAccessor.CornerText.gameObject.SetActive(true);
            uiAccessor.CornerText.text = "0";
            
            stackEnchantmentLogic.Subscribe(OnStacksChange);
        }

        private void OnStacksChange(int newValue)
        {
            _accessor.CornerText.text = newValue.ToString();
        }
    }
}