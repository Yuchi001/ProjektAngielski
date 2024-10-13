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

        public void Setup(EnchantmentBase pickedEnchant)
        {
            var data = pickedEnchant.Get();
            enchantDescription.text = pickedEnchant.GetDescriptionText();
            enchantImage.sprite = data.EnchantmentSprite;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(GameManager.AcceptBind)) return;
            
            AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
            Time.timeScale = 1;
            PlayerManager.Instance.PlayerMovement.ResetKeys();
            Destroy(gameObject);
        }
    }
}