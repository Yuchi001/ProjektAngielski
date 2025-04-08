using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers.Enums;
using Other.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using TargetSearchPack;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class FireWandLogic : ItemLogicBase
    {
        [SerializeField] private GameObject flightParticles;
        [SerializeField] private GameObject onHitParticles;

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
                var target = TargetManager.FindTarget(FindStrategy, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                
                projectile.Setup(Damage, Speed)
                    .SetTarget(target.transform)
                    .SetOnHitParticles(onHitParticles)
                    .SetOnHitAction(OnHitAction)
                    .SetFlightParticles(flightParticles)
                    .SetEffect(EEffectType.Burn, EffectDuration)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private void OnHitAction(GameObject enemy, Projectile projectile)
        {
            var range = GetStatValue(EItemSelfStatType.BlastRange);
            var damage = GetStatValueAsInt(EItemSelfStatType.BlastDamage);
            
            if (this != null) StartCoroutine(BoomCoroutine(enemy, range, damage));
        }

        private IEnumerator BoomCoroutine(GameObject impactEnemy, float range, int damage)
        {
            var enemyInstanceId = impactEnemy.GetInstanceID();
            var results = new Collider2D[50];
            var position = impactEnemy.transform.position;
            Physics2D.OverlapCircleNonAlloc(position, range, results);
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range);
            AudioManager.PlaySound(ESoundType.BananaBoom);
            foreach (var hitCollider in results)
            {
                if(hitCollider == null) continue;
                
                if(!hitCollider.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                if(enemy.GetInstanceID() == enemyInstanceId) continue;
                
                var particles = Instantiate(onHitParticles, enemy.transform.position, Quaternion.identity);
                Destroy(particles, 2f);
                
                enemy.GetDamaged(damage);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}