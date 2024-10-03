﻿using System;
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

            if (!_selected || !Input.GetKeyDown(KeyCode.Space)) return;
            
            OnClick();
        }

        public void Setup(SoWeapon weapon, GameObject levelUpUiGameObject, int index)
        {
            _index = index;
            _weapon = weapon;
            _levelUpUiGameObject = levelUpUiGameObject;
            weapon.GenerateNextLevelStats();
            weaponDescriptionField.text = PlayerWeaponry.GetWeaponDescription(weapon);
            enchantmentLevelField.text = $"Lvl {weapon.GetNextLevelEnchantmentLevel()}";
            weaponImage.sprite = weapon.WeaponSprite;
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