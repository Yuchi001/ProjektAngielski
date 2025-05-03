using System.Globalization;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public sealed class CdrDisplayStrategy : EnchantmentDisplayStrategy
    {
        private SoCooldownEnchantment.CooldownData _data;
        private EnchantmentItemAccessor _accessor;
        
        public override void SetDisplayData(EnchantmentLogic logicBase, EnchantmentItemAccessor uiAccessor)
        {
            base.SetDisplayData(logicBase, uiAccessor);
            _accessor = uiAccessor;
            _data = logicBase.GetData<SoCooldownEnchantment.CooldownData>();
            
            uiAccessor.MainText.gameObject.SetActive(true);
            uiAccessor.SecondaryImage.gameObject.SetActive(true);
            
            uiAccessor.MainText.text = "";
            uiAccessor.SecondaryImage.fillAmount = 0;
        }

        private void Update()
        {
            var percentage = _data.GetPercentage();
            if (percentage >= 0.99f)
            {
                _accessor.MainText.text = "";
                return;
            }
            
            var value = Mathf.Round(percentage * 10f) / 10f; 
            _accessor.MainText.text = value % 1 == 0 
                ? ((int)value).ToString(CultureInfo.InvariantCulture)
                : value.ToString("0.0", CultureInfo.InvariantCulture);
        }
    }
}