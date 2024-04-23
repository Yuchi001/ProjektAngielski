using System;
using System.Collections.Generic;
using System.Linq;
using PlayerPack;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelUpUi : MonoBehaviour
    {
        [SerializeField] private GameObject weaponSlotPrefab;
        [SerializeField] private RectTransform weaponContainer;
        [SerializeField] private int randomWeaponsCount = 3;

        private int _pickedWeaponIndex = 0;

        public delegate void SelectDelegate(int index);
        public static event SelectDelegate OnSelect;

        public void Setup()
        {
            Time.timeScale = 0;

            var index = 0;
            var weapons = PlayerManager.Instance.PlayerWeaponry.GetRandomWeapons(randomWeaponsCount);
            foreach (var weapon in weapons)
            {
                var slot = Instantiate(weaponSlotPrefab, weaponContainer.position, Quaternion.identity,
                    weaponContainer);
                var slotScript = slot.GetComponent<WeaponSlotUi>();
                slotScript.Setup(weapon, gameObject, index);
                index++;
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(weaponContainer);
            
            OnSelect?.Invoke(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _pickedWeaponIndex--;
                if (_pickedWeaponIndex < 0) _pickedWeaponIndex = randomWeaponsCount - 1;
                OnSelect?.Invoke(_pickedWeaponIndex);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                _pickedWeaponIndex++;
                if (_pickedWeaponIndex >= randomWeaponsCount) _pickedWeaponIndex = 0;
                OnSelect?.Invoke(_pickedWeaponIndex);
            }
        }
    }
}