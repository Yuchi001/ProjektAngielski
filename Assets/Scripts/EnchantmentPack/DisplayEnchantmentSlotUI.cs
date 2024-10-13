using System;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    public class DisplayEnchantmentSlotUI : MonoBehaviour
    {
        [SerializeField] private Image enchantmentImage;
        [SerializeField] private TextMeshProUGUI enchantmentStackCount;

        private SoEnchantment _enchantmentData;

        private IStackEnchantment _stackEnchantment;
        private ICooldownEnchantment _cooldownEnchantment;
        
        public void Setup(EnchantmentBase enchantmentBase)
        {
            _enchantmentData = enchantmentBase.Get();
            enchantmentImage.sprite = _enchantmentData.EnchantmentSprite;
            enchantmentImage.fillAmount = 1;
            
            switch (_enchantmentData.EEnchantmentType)
            {
                case EEnchantmentType.None:
                    break;
                case EEnchantmentType.Stack:
                    _stackEnchantment = enchantmentBase.GetComponent<IStackEnchantment>();
                    break;
                case EEnchantmentType.Cooldown:
                    _cooldownEnchantment = enchantmentBase.GetComponent<ICooldownEnchantment>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            enchantmentStackCount.gameObject.SetActive(_enchantmentData.EEnchantmentType == EEnchantmentType.Stack);
        }

        private void Update()
        {
            switch (_enchantmentData.EEnchantmentType)
            {
                case EEnchantmentType.None:
                    break;
                case EEnchantmentType.Stack:
                    enchantmentStackCount.text = _stackEnchantment.Stacks.ToString();
                    break;
                case EEnchantmentType.Cooldown:
                    var currentTime = _cooldownEnchantment.CurrentTime;
                    var isActive = _cooldownEnchantment.IsActive;
                    enchantmentImage.fillAmount = isActive && currentTime < 1 ? 1 : currentTime;

                    if (_enchantmentData.EnchantmentActiveSprite == null) break;

                    enchantmentImage.sprite =
                        isActive ? _enchantmentData.EnchantmentActiveSprite : _enchantmentData.EnchantmentSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}