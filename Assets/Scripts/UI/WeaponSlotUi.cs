using System;
using System.Collections.Generic;
using Managers;
using PlayerPack;
using PlayerPack.PlayerMovementPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeaponPack.SO;

namespace UI
{
    public class WeaponSlotUi : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponDescriptionField;
        [SerializeField] private TextMeshProUGUI enchantmentLevelField;

        private PlayerWeaponry PlayerWeaponry => PlayerManager.Instance.PlayerWeaponry;
        private PlayerMovement PlayerMovement => PlayerManager.Instance.PlayerMovement;
        private SoWeapon _weapon;
        private int _index;
        private GameObject _levelUpUiGameObject;
        private bool _selected = false;

        private void Awake()
        {
            LevelUpUi.OnSelect += OnSelect;
        }

        private void Update()
        {
            var scale = _selected ? 1.1f : 1;
            transform.localScale = new Vector3(scale, scale, scale);

            if (!_selected || !Input.GetKeyDown(GameManager.AcceptBind)) return;
            
            OnClick();
        }

        public void Setup(SoWeapon weapon, GameObject levelUpUiGameObject, int index)
        {
            _index = index;
            _weapon = weapon;
            _levelUpUiGameObject = levelUpUiGameObject;
            weapon.GenerateNextLevelStats();
            weaponDescriptionField.text = PlayerWeaponry.GetWeaponDescription(weapon);
            enchantmentLevelField.text = ArabicToRoman(weapon.GetNextLevelEnchantmentLevel());
            weaponImage.sprite = weapon.WeaponSprite;
        }

        private string ArabicToRoman(int input)
        {
            var numberPairList = new List<(int arabic, char roman)>()
            {
                (arabic: 10, roman: 'X'),
                (arabic: 5, roman: 'V'),
                (arabic: 1, roman: 'I')
            };
            var roman = "";

            foreach (var numberPair in numberPairList)
            {
                while (input - numberPair.arabic >= 0)
                {
                    input -= numberPair.arabic;
                    roman += numberPair.roman;
                }
 
                if (input + 1 != numberPair.arabic || numberPair.arabic == 1) continue;

                roman += $"I{numberPair.roman}";
                break;
            }

            return roman;
        }

        public void OnClick()
        {
            Time.timeScale = 1;
            PlayerWeaponry.AddWeapon(_weapon);
            Destroy(_levelUpUiGameObject);
            PlayerMovement.ResetKeys();
        }

        public void OnSelect(int index)
        {
            _selected = _index == index;
        }

        private void OnDisable()
        {
            LevelUpUi.OnSelect -= OnSelect;
        }
    }
}