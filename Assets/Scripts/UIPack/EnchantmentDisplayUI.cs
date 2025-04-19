using System;
using System.Collections;
using System.Text.RegularExpressions;
using EnchantmentPack;
using EnchantmentPack.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPack
{
    public class EnchantmentDisplayUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private Image image;

        public override void OnOpen(string key)
        {
            base.OnOpen(key);
            Time.timeScale = 0;
        }

        public void SetData(SoEnchantment enchantment)
        {
            image.sprite = enchantment.Sprite;
            descriptionField.text = enchantment.GetDescription();
            var original = enchantment.Name;
            var result = Regex.Replace(original, "(?<!^)([A-Z])", " $1");
            nameField.text = $"{result} {ToRomanNumeral(enchantment.Level)}";
        }

        protected override void OnDeactivate()
        {
            Time.timeScale = 1;
            base.OnDeactivate();
        }

        
        private static string ToRomanNumeral(int level)
        {
            return level switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                5 => "V",
                _ => throw new ArgumentOutOfRangeException(nameof(level), "Level must be between 1 and 5.")
            };
        }
    }
}