using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using ItemPack.WeaponPack.Other;
using ItemPack.WeaponPack.SideClasses;
using Managers;
using Managers.Other;
using PlayerPack;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack
{
    public abstract class ItemLogicBase : MonoBehaviour
    {
        public int Level { get; private set; } = 0;
        public float TimerScaled => 1 - _timer / Cooldown;
        public int Damage => (int)(GetStatValue(EItemSelfStatType.Damage) ?? 0);
        protected float Speed => GetStatValue(EItemSelfStatType.ProjectileSpeed) ?? 0;
        protected int ProjectileCount => (int)(GetStatValue(EItemSelfStatType.ProjectilesCount) ?? 0);
        protected float PushForce => GetStatValue(EItemSelfStatType.PushForce) ?? 0;
        protected float Cooldown => (GetStatValue(EItemSelfStatType.Cooldown) ?? 1) * CustomCooldownModifier();

        protected static PlayerEnchantments PlayerEnchantments => GameManager.Instance.CurrentPlayer.PlayerEnchantments;
        protected static Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected static Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;
        protected static Projectile Projectile => GameManager.Instance.GetPrefab<Projectile>(PrefabNames.Projectile);
        public SoItem Item => _item;
        private SoItem _item;

        private float _timer = 0;
        private bool spawned = false;
        
        public void Setup(SoItem item, int level)
        {
            _item = Instantiate(item);
            Level = level;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < Cooldown || (spawned && _item.OneTimeSpawnLogic)) return;
            
            spawned = Use();
            _timer = spawned ? 0 : Cooldown;
        }
 
        protected abstract bool Use();

        protected float? GetStatValue(EItemSelfStatType statTypeType)
        {
            var stat = Item.GetStat(statTypeType, Level);
            return stat?.StatValue;
        }
        
        protected virtual float CustomCooldownModifier()
        {
            return 1; 
        }
    }
}