using System.Collections.Generic;
using AudioPack;
using EnemyPack;
using ItemPack.Enums;
using Managers.Enums;
using Other;
using PlayerPack;
using ProjectilePack;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class EatenBananaLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject boomParticles;

        private float BlastRange => GetStatValue(EItemSelfStatType.BlastRange);
        private List<EnemyLogic> _cachedTargetList = new();

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.BlastRange,
        };
        
        protected override bool Use()
        {
            ProjectileManager.SpawnProjectile(IProjectileMovementStrategy.IGNORE, this)
                .SetSprite(projectileSprite)
                .SetScale(0.5f)
                .SetOnHitAction(OnHit)
                .Ready();

            return true;
        }

        private bool OnHit(Projectile projectile, CanBeDamaged hitObj)
        {
            AudioManager.PlaySound(ESoundType.BananaBoom);

            var projectilePos = projectile.transform.position;
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionBig, projectilePos, BlastRange);

            var found = TargetDetector.TryGetEnemiesInRange(projectilePos, BlastRange, ref _cachedTargetList);
            if (!found) return false;
            
            foreach (var enemy in _cachedTargetList)
            {
                var damageContext = PlayerManager.GetDamageContextManager()
                    .GetDamageContext(Damage, projectile, hitObj, InventoryItem.ItemTags);
                enemy.GetDamaged(damageContext.Damage);
            }
            _cachedTargetList.Clear();

            return false;
        }
    }
}