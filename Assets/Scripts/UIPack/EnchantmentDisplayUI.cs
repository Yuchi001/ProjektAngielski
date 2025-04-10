using System;
using System.Collections;
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

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(animTime);
            Time.timeScale = 0;
        }

        public void SetData(SoEnchantment enchantment)
        {
            image.sprite = enchantment.EnchantmentSprite;
            textField.text = enchantment.GetDescription();
        }

        public override void OnClose()
        {
            Time.timeScale = 1;
            base.OnClose();
        }
    }
}