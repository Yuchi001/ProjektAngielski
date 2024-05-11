using System;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeaponPack;

namespace UI
{
    public class DisplayWeaponSlotUi : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI timerTextField;

        private WeaponLogicBase _weaponLogicBase;
        private bool _ready = false;

        public void Setup(WeaponLogicBase weaponLogicBase)
        {
            _weaponLogicBase = weaponLogicBase;
            weaponImage.sprite = _weaponLogicBase.Weapon.WeaponSprite;
            frameImage.color = _weaponLogicBase.Weapon.WeaponColor;

            _ready = true;
        }

        private void Update()
        {
            if (!_ready) return;
            
            timerTextField.text = _weaponLogicBase.Cooldown > 0.3f ? $"{_weaponLogicBase.Cooldown - _weaponLogicBase.CurrentTimer:0.0}" : "Max.";
            fillImage.fillAmount = _weaponLogicBase.Cooldown > 0.3f ? 1 - _weaponLogicBase.CurrentTimer / _weaponLogicBase.Cooldown : 1;
        }
    }
}