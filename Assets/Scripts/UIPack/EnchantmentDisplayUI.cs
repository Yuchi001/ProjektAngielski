using EnchantmentPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPack
{
    public class EnchantmentDisplayUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Image image;
        
        public void SetData(SoEnchantment enchantment)
        {
            image.sprite = enchantment.EnchantmentSprite;
            textField.text = enchantment.GetDescription();
        }
    }
}