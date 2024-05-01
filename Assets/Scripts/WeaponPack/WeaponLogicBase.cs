using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using PlayerPack;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

        public float Cooldown => GetStatValue(EWeaponStat.CooldownReduction) ?? 1;
        private float _timer = 0;
        
        protected int _level = 0;

        protected bool spawned = false;

        protected List<WeaponStatPair> OngoingStats = new();


        public float CurrentTimer => _timer;
        
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
         // TEST
        protected abstract void UseWeapon();

        private void OnLevelUp(string weaponName)
        {
            if (weaponName != _weapon.WeaponName) return;
            
            IncrementStats();
        }

        private void IncrementStats()
        {
            var weaponStats = _weapon.WeaponUpgradeStats;
            var currentWeaponStats = weaponStats[_level % weaponStats.Count].levelStats;
            foreach (var stat in currentWeaponStats)
            {
                var statTuple = OngoingStats.FirstOrDefault(s => s.statType == stat.statType);
                if(statTuple == default) continue;

                if (stat.isPercentage) statTuple.statValue *= 1 + stat.statValue;
                else statTuple.statValue += stat.statValue;
            }
            _level++;
        }

        protected float? GetStatValue(EWeaponStat statType)
        {
            var stat = _realWeaponStats.FirstOrDefault(s => s.statType == statType);
            return stat?.statValue;
        }
    }
}