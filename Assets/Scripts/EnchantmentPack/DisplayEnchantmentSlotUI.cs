using System;
using EnchantmentPack.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    public class DisplayEnchantmentSlotUI : MonoBehaviour
    {
        [SerializeField] private Image enchantmentImage;
        
        private ICooldownEnchantment _cooldownEnchantment;

        private SoEnchantment _enchantmentData;
        
        public void Setup(EnchantmentBase enchantmentBase)
        {
            _enchantmentData = enchantmentBase.Get();

            if (enchantmentBase.TryGetComponent<ICooldownEnchantment>(out var cooldownEnchantment))
            {
                _cooldownEnchantment = cooldownEnchantment;
                if (!_enchantmentData.HasCooldown) 
                    throw new Exception("Enchantment wasn't marked with HasCooldown, but it's behaviour implements ICooldownEnchantment interface!");
            }

            if (_enchantmentData.HasCooldown && _cooldownEnchantment == null)
                throw new Exception(
                    "Enchantment was marked with HasCooldown, but it's behaviour didn't implement ICooldownEnchantment");

            enchantmentImage.sprite = _enchantmentData.EnchantmentSprite;
            enchantmentImage.fillAmount = 1;
        }

        private void Update()
        {
            if (_cooldownEnchantment == null) return;

            var currentTime = _cooldownEnchantment.CurrentTime;
            var isActive = _cooldownEnchantment.IsActive;
            enchantmentImage.fillAmount = isActive && currentTime < 1 ? 1 : currentTime;

            if (_enchantmentData.EnchantmentActiveSprite == null) return;

            enchantmentImage.sprite =
                isActive ? _enchantmentData.EnchantmentActiveSprite : _enchantmentData.EnchantmentSprite;
        }
    }
}