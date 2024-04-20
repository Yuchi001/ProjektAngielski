using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaponPack;
using WeaponPack.SO;

namespace PlayerPack
{
    public class PlayerWeaponry : MonoBehaviour
    {
        [SerializeField] private int maxWeaponsInEq = 4;

        private List<SoWeapon> currentWeapons = new();

        public delegate void WeaponLevelUpDelegate(string weaponName);
        public static event WeaponLevelUpDelegate OnWeaponLevelUp;

        public void AddWeapon(SoWeapon weaponToAdd)
        {
            var weapon = currentWeapons.FirstOrDefault(w => w.WeaponName == weaponToAdd.WeaponName);
            if (weapon != default) OnWeaponLevelUp?.Invoke(weaponToAdd.WeaponName);
            
            var weaponLogicObj = Instantiate(weaponToAdd.WeaponLogicPrefab, transform, true);
            weaponLogicObj.transform.localPosition = Vector3.zero;
            weaponLogicObj.GetComponent<WeaponLogicBase>().Setup(weaponToAdd);
            currentWeapons.Add(weaponToAdd);
        }
    }
}