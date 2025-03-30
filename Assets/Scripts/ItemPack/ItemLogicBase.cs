using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using PlayerPack;
using UnityEngine;

namespace ItemPack
{
    public abstract class ItemLogicBase : MonoBehaviour
    {
        public int Level { get; private set; } = 0;
        public float TimerScaled => 1 - _timer / Cooldown;
        public int Damage => GetStatValueAsInt(EItemSelfStatType.Damage);
        protected float Speed => GetStatValue(EItemSelfStatType.ProjectileSpeed);
        protected int ProjectileCount => GetStatValueAsInt(EItemSelfStatType.ProjectilesCount);
        protected float PushForce => GetStatValue(EItemSelfStatType.PushForce);
        protected float Cooldown => GetStatValue(EItemSelfStatType.Cooldown) * CustomCooldownModifier();

        protected static PlayerEnchantments PlayerEnchantments => GameManager.Instance.CurrentPlayer.PlayerEnchantments;
        protected static Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        protected static Transform PlayerTransform => GameManager.Instance.CurrentPlayer.transform;
        protected static Projectile Projectile => GameManager.Instance.GetPrefab<Projectile>(PrefabNames.Projectile);
        public SoInventoryItem InventoryItem => _inventoryItem;
        private SoInventoryItem _inventoryItem;

        private float _timer = 0;
        private bool spawned = false;

        /// <summary>
        /// Remember to override GetUseStats to have full control over returned stats. <para /> 
        /// Base implementation includes <b>DAMAGE</b> and <b>COOLDOWN</b>. <para /> 
        /// Other used stats that are not included are:
        /// <ul>
        /// <li><b>PUSH FORCE</b></li>
        /// <li><b>PROJECTILE COUNT</b></li>
        /// <li><b>SPEED</b></li>
        /// </ul>
        /// To get all three of them use <i><see cref="_otherDefaultStats"/></i> or <i><see cref="_otherDefaultStatsNoPush"/></i>
        /// </summary>
        protected abstract List<EItemSelfStatType> UsedStats { get; }

        protected readonly List<EItemSelfStatType> _otherDefaultStats = new()
        {
            EItemSelfStatType.PushForce,
            EItemSelfStatType.ProjectilesCount,
            EItemSelfStatType.ProjectileSpeed
        };
        
        protected readonly List<EItemSelfStatType> _otherDefaultStatsNoPush = new()
        {
            EItemSelfStatType.ProjectilesCount,
            EItemSelfStatType.ProjectileSpeed
        };

        private readonly List<EItemSelfStatType> _baseStats = new()
        {
            EItemSelfStatType.Damage,
            EItemSelfStatType.Cooldown
        };

        public virtual IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return UsedStats.Concat(_baseStats);
        }

        public void Setup(SoInventoryItem inventoryItem, int level)
        {
            _inventoryItem = Instantiate(inventoryItem);
            Level = level;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < Cooldown || (spawned && _inventoryItem.OneTimeSpawnLogic)) return;
            
            spawned = Use();
            _timer = spawned ? 0 : Cooldown;
        }
 
        protected abstract bool Use();

        protected float GetStatValue(EItemSelfStatType statTypeType)
        {
            return InventoryItem.GetStatValue(statTypeType, Level);
        }
        
        protected int GetStatValueAsInt(EItemSelfStatType statTypeType)
        {
            return (int)InventoryItem.GetStatValue(statTypeType, Level);
        }
        
        protected virtual float CustomCooldownModifier()
        {
            return 1; 
        }
    }
}