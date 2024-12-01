using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using PlayerPack;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.SideClasses;
using WeaponPack.SO;

namespace WeaponPack
{
    public abstract class WeaponLogicBase : MonoBehaviour
    {
        public SoWeapon Weapon => _weapon;
        public int Level => _level;

        private SoWeapon _weapon;

        private List<WeaponStatPair> _realWeaponStats = new();

        public int Damage => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.Damage)!.StatValue;
        protected float Speed => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectileSpeed)!.StatValue;
        protected int ProjectileCount => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectilesCount)!.StatValue;
        protected float PushForce => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.PushForce)!.StatValue;

        protected PlayerEnchantments PlayerEnchantments =>
            GameManager.Instance.CurrentPlayer.PlayerEnchantments;
        protected Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;

        public float TimerScaled => 1 - CurrentTimer / Cooldown;

        protected virtual float CustomCooldownModifier()
        {
            return 1; 
        }
        public float Cooldown => (GetStatValue(EWeaponStat.CooldownReduction) ?? 1) * CustomCooldownModifier();
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
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < Cooldown || (spawned && _weapon.OneTimeSpawnLogic)) return;
            
            spawned = UseWeapon();
            _timer = spawned ? 0 : Cooldown;
        }
 
        protected abstract bool UseWeapon();

        protected float? GetStatValue(EWeaponStat statType)
        {
            var stat = _realWeaponStats.FirstOrDefault(s => s.StatType == statType);
            return stat?.StatValue;
        }
    }
}