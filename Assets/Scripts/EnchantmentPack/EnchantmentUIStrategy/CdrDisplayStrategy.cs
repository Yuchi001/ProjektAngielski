using System;
using System.Globalization;
using EnchantmentPack.EnchantmentLogic;
using UnityEngine;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public sealed class CdrDisplayStrategy : EnchantmentDisplayStrategy
    {
        private CooldownEnchantmentLogic _logic;
        private EnchantmentItemAccessor _accessor;
        
        public override void SetLogic(EnchantmentLogicBase logicBase, EnchantmentItemAccessor uiAccessor)
        {
            base.SetLogic(logicBase, uiAccessor);
            _accessor = uiAccessor;
            _logic = (CooldownEnchantmentLogic)logicBase;
            
            uiAccessor.MainText.gameObject.SetActive(true);
            uiAccessor.SecondaryImage.gameObject.SetActive(true);
            
            uiAccessor.MainText.text = "";
            uiAccessor.SecondaryImage.fillAmount = 0;
        }

        private void Update()
        {
            var percentage = _logic.CooldownPercentage;
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