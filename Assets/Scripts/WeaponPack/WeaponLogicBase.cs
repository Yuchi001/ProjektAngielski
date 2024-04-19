using System;
using System.Collections.Generic;
using System.Linq;
using PlayerPack;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.SO;

namespace WeaponPack
{
    public abstract class WeaponLogicBase : MonoBehaviour
    {
        protected SoWeapon _weapon;

        protected List<WeaponStatPair> _realWeaponStats = new();

        private float Cooldown => _realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.Cooldown)!.statValue;
        private float _timer = 0;
        
        protected int _level = 0;
        
        public void Setup(SoWeapon weapon)
        {
            _weapon = weapon;
            _realWeaponStats = weapon.WeaponStartingStats;
            PlayerWeaponry.OnWeaponLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            PlayerWeaponry.OnWeaponLevelUp -= OnLevelUp;
        }

        private void Update()
        {
            if (_timer < Cooldown) return;

            _timer = 0;
            UseWeapon();
        }

        protected virtual void UseWeapon()
        {
            
        }

        private void OnLevelUp(string weaponName)
        {
            if (weaponName != _weapon.WeaponName) return;

            IncrementStats();
        }

        private void IncrementStats()
        {
            var stats = _weapon.WeaponStartingStats;
            foreach (var stat in GetCurrentUpgradeStats())
            {
                var statTuple = stats.FirstOrDefault(s => s.statType == stat.statType);
                if(statTuple == default) continue;

                if (stat.isPercentage) statTuple.statValue *= 1 - stat.statValue;
                else statTuple.statValue += stat.statValue;
            }
        }

        private List<WeaponStatPair> GetCurrentUpgradeStats()
        {
            for (var i = _weapon.WeaponUpgradeStats.Count; i > 0; i--)
            {
                if (_level % i == 0) return _weapon.WeaponUpgradeStats[i].levelStats;
            }

            return new List<WeaponStatPair>();
        }
    }
}