using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using ItemPack.WeaponPack.SideClasses;
using Managers;
using PlayerPack;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack
{
    public abstract class ItemLogicBase : MonoBehaviour
    {
        public int Level { get; private set; } = 0;
        public float TimerScaled => 1 - _timer / Cooldown;
        public int Damage => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.Damage)!.StatValue;
        protected float Speed => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectileSpeed)!.StatValue;
        protected int ProjectileCount => (int)_realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.ProjectilesCount)!.StatValue;
        protected float PushForce => _realWeaponStats.FirstOrDefault(s => s.StatType == EWeaponStat.PushForce)!.StatValue;
        protected float Cooldown => (GetStatValue(EWeaponStat.CooldownReduction) ?? 1) * CustomCooldownModifier();

        protected static PlayerEnchantments PlayerEnchantments => GameManager.Instance.CurrentPlayer.PlayerEnchantments;
        protected static Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected static Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;
        
        public SoItem Item => _item;
        private SoItem _item;

        private float _timer = 0;
        private bool spawned = false;


        private List<StatPair> OngoingStats = new();
        private List<StatPair> _realWeaponStats = new();
        
        
        public void Setup(SoItem item)
        {
            _item = Instantiate(item);
            OngoingStats = _item.StartingStats;
            _realWeaponStats = _item.StartingStats;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < Cooldown || (spawned && _item.OneTimeSpawnLogic)) return;
            
            spawned = Use();
            _timer = spawned ? 0 : Cooldown;
        }
 
        protected abstract bool Use();

        protected float? GetStatValue(EWeaponStat statType)
        {
            var stat = _realWeaponStats.FirstOrDefault(s => s.StatType == statType);
            return stat?.StatValue;
        }
        
        protected virtual float CustomCooldownModifier()
        {
            return 1; 
        }

        public virtual void LevelUp()
        {
            Level++;
        }
    }
}