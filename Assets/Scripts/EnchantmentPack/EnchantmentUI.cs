using System;
using Managers;
using Managers.Enums;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    public class EnchantmentUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI enchantDescription;
        [SerializeField] private Image enchantImage;

        private SoEnchantment _pickedEnchantment;

        public void Setup(SoEnchantment pickedEnchant)
        {
            _pickedEnchantment = pickedEnchant;
            enchantDescription.text = pickedEnchant.GetDescription();
            enchantImage.sprite = pickedEnchant.EnchantmentSprite;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(GameManager.AcceptBind)) return;
            
            AudioManager.Instance.PlaySound(ESoundType.PowerUp);
            Time.timeScale = 1;
            PlayerManager.Instance.PlayerEnchantments.AddEnchantment(_pickedEnchantment);
            PlayerManager.Instance.PlayerMovement.ResetKeys();
            Destroy(gameObject);
        }
    }
}