using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaponPack;
using WeaponPack.SO;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerWeaponry : MonoBehaviour
    {
        private List<WeaponLogicBase> _currentWeapons = new();
        private List<SoWeapon> _allWeapons = new();

        public delegate void WeaponAddDelegate(WeaponLogicBase weaponLogicBase);
        public static event WeaponAddDelegate OnWeaponAdd;

        public delegate void WeaponLevelUpDelegate(string weaponName);
        public static event WeaponLevelUpDelegate OnWeaponLevelUp;

        private void Awake()
        {
            _allWeapons = Resources.LoadAll<SoWeapon>("Weapons").Select(Instantiate).ToList();
        }

        public void AddWeapon(SoWeapon weaponToAdd)
        {
            var weapon = _currentWeapons.FirstOrDefault(w => w.Weapon.WeaponName == weaponToAdd.WeaponName);
            if (weapon != default)
            {
                OnWeaponLevelUp?.Invoke(weaponToAdd.WeaponName);
                return;
            }
            
            var weaponLogicObj = Instantiate(weaponToAdd.WeaponLogicPrefab, transform, true);
            weaponLogicObj.transform.localPosition = Vector3.zero;
            var weaponLogic = weaponLogicObj.GetComponent<WeaponLogicBase>();
            weaponLogic.Setup(weaponToAdd);
            _currentWeapons.Add(weaponLogic);
            
            OnWeaponAdd?.Invoke(weaponLogic);
        }

        public string GetWeaponDescription(SoWeapon weapon)
        {
            var currentWeapon = _currentWeapons.FirstOrDefault(w => w.Weapon.WeaponName == weapon.WeaponName);
            if (currentWeapon == default) return "<color=red>NEW!</color> " + weapon.WeaponDescription;
            
            var level = currentWeapon.Level;
            return $"<color=green>Next level {level + 2}:</color> " + currentWeapon.Weapon.GetNextLevelDescription();
        }

        public int GetNextLevelEnchantmentLevel(SoWeapon weapon)
        {
            var currentWeapon = _currentWeapons.FirstOrDefault(w => w.Weapon.WeaponName == weapon.WeaponName);
            return currentWeapon == default ? 0 : currentWeapon.Weapon.GetNextLevelEnchantmentLevel();
        }

        public IEnumerable<SoWeapon> GetRandomWeapons(int count)
        {
            var weapons = new List<SoWeapon>();
            var weaponPool = new List<SoWeapon>(_allWeapons);
            if (_currentWeapons.Count >= PlayerManager.Instance.PickedCharacter.MaxWeaponsInEq)
            {
                var weaponNames = _currentWeapons.Select(w => w.Weapon.WeaponName);
                weaponPool = weaponPool.Where(w => weaponNames.Contains(w.WeaponName)).ToList();
            }

            for (var i = 0; i < count; i++)
            {
                if (weaponPool.Count == 0) break;
                
                var randomIndex = Random.Range(0, weaponPool.Count);
                weapons.Add(weaponPool[randomIndex]);
                weaponPool.RemoveAt(randomIndex);
            }
            
            return weapons;
        }

        public void DestroyAllWeapons()
        {
            foreach (var weapon in _currentWeapons)
            {
                if (weapon == null) continue;
                Destroy(weapon.gameObject);
            }
        }
    }
}