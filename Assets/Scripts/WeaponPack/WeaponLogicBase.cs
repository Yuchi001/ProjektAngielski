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

        protected int Damage => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.Damage)!.StatValue;
        protected float Speed => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectileSpeed)!.StatValue;
        protected int ProjectileCount => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectilesCount)!.StatValue;
        protected float PushForce => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.PushForce)!.StatValue;

        protected PlayerEnchantmentManager PlayerEnchantmentManager =>
            GameManager.Instance.CurrentPlayer.PlayerEnchantmentManager;
        protected Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;

        public float TimerScaled => 1 - CurrentTimer / Cooldown;
        public float Cooldown => GetStatValue(EWeaponStat.CooldownReduction) ?? 1;
        private float _timer = 0;

        private int _level = 0;

        private bool spawned = false;

        private List<WeaponStatPair> OngoingStats = new();


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
            
            spawned = UseWeapon();
            _timer = spawned ? 0 : Cooldown;
        }
 
        protected abstract bool UseWeapon();

        private void OnLevelUp(SoWeapon weapon)
        {
            if (weapon.WeaponName != _weapon.WeaponName) return;
            
            IncrementStats(weapon);
        }

        private void IncrementStats(SoWeapon weapon)
        {
            foreach (var stat in weapon.GetNextLevelStats())
            {
                var statTuple = OngoingStats.FirstOrDefault(s => s.StatType == stat.StatType);
                if(statTuple == default) continue;

                if (stat.IsPercentage) statTuple.SetStatValue(statTuple.StatValue * (1 + stat.StatValue));
                else statTuple.SetStatValue(statTuple.StatValue + stat.StatValue);
            }
            _level++;

            spawned = !_weapon.OneTimeSpawnLogic;
        }

        protected float? GetStatValue(EWeaponStat statType)
        {
            var stat = _realWeaponStats.FirstOrDefault(s => s.StatType == statType);
            return stat?.StatValue;
        }
    }
}