using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using EnemyPack;
using ItemPack.Enums;
using Managers.Enums;
using NUnit.Framework;
using Other;
using Other.Enums;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class FireWandLogic : ItemLogicBase
    {
        [SerializeField] private GameObject flightParticles;
        [SerializeField] private GameObject onHitParticles;
        [SerializeField] private Sprite projectileSprite;

        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.EffectDuration,
            EItemSelfStatType.BlastRange,
            EItemSelfStatType.BlastDamage
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        private NearPlayerStrategy _findStrategy;
        private NearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new NearPlayerStrategy();
            }
        }

        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = TargetManager.FindTarget(FindStrategy, 20f, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectileMovementStrategy = new DirectionMovementStrategy(PlayerPos, 
                    target.transform.position, Speed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetOnHitAction(OnHitAction)
                    .SetEffect(EEffectType.Burn, EffectDuration)
                    .SetSprite(projectileSprite)
                    .SetScale(0.3f)
                    .Ready(); // TODO: flight particles
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private bool OnHitAction(Projectile projectile, CanBeDamaged enemy)
        {
            var range = GetStatValue(EItemSelfStatType.BlastRange);
            var damage = GetStatValueAsInt(EItemSelfStatType.BlastDamage);
            
            if (this != null) StartCoroutine(BoomCoroutine(enemy, projectile, range, damage));

            return false;
        }

        private IEnumerator BoomCoroutine(CanBeDamaged impactEnemy, Projectile projectile, float range, int damage)
        {
            var enemyInstanceId = impactEnemy.GetInstanceID();
            var position = impactEnemy.transform.position;
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range);
            AudioManager.PlaySound(ESoundType.BananaBoom);
            var results = new List<EnemyLogic>(TargetDetector.EnemiesInRange(position, range));
            foreach (var enemy in results)
            {
                if(enemy.GetInstanceID() == enemyInstanceId) continue;
                
                var particles = Instantiate(onHitParticles, enemy.transform.position, Quaternion.identity);
                Destroy(particles, 2f);
                
                var damageContext = PlayerManager.GetDamageContextManager()
                    .GetDamageContext(damage, projectile, enemy, InventoryItem.ItemTags);
                enemy.GetDamaged(damageContext.Damage);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}