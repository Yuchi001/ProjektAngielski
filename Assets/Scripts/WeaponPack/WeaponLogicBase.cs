using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using PlayerPack;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.SO;

namespace WeaponPack
{
    public abstract class WeaponLogicBase : MonoBehaviour
    {
        public SoWeapon Weapon => _weapon;
        public int Level => _level;

        private SoWeapon _weapon;

        private List<WeaponStatPair> _realWeaponStats = new();

        protected int Damage => (int)_realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.Damage)!.statValue;
        protected float Speed => _realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.ProjectileSpeed)!.statValue;
        protected int ProjectileCount => (int)_realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.ProjectilesCount)!.statValue;
        
        protected Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;

        protected float Cooldown => _weapon.Cooldown * (1 - _realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.CooldownReduction)!.statValue);
        private float _timer = 0;
        
        protected int _level = 0;

        protected bool spawned = false;

        protected List<WeaponStatPair> OngoingStats = new();
        
        public void Setup(SoWeapon weapon)
        {
            _weapon = Instantiate(weapon);
            OngoingStats = _weapon.WeaponStartingStats;
            _realWeaponStats = _weapon.WeaponStartingStats;
            PlayerWeaponry.OnWeaponLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            PlayerWeaponry.OnWeaponLevelUp -= OnLevelUp;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < Cooldown || (spawned && _weapon.OneTimeSpawnLogic)) return;

            _timer = 0;
            UseWeapon();
        }

        protected abstract void UseWeapon();

        private void OnLevelUp(string weaponName)
        {
            if (weaponName != _weapon.WeaponName) return;

            _level++;
            IncrementStats();
        }

        private void IncrementStats()
        {
            foreach (var stat in GetCurrentUpgradeStats())
            {
                var statTuple = OngoingStats.FirstOrDefault(s => s.statType == stat.statType);
                if(statTuple == default) continue;

                if (stat.isPercentage) statTuple.statValue *= 1 + stat.statValue;
                else statTuple.statValue += stat.statValue;
            }
        }

        private List<WeaponStatPair> GetCurrentUpgradeStats()
        {
            for (var i = 0; i < _weapon.WeaponUpgradeStats.Count; i++)
            {
                if ((_level + _weapon.WeaponUpgradeStats.Count) % (Mathf.Abs(i - _weapon.WeaponUpgradeStats.Count)) == 1) 
                    return _weapon.WeaponUpgradeStats[i].levelStats;
            }

            return new List<WeaponStatPair>();
        }
    }
}