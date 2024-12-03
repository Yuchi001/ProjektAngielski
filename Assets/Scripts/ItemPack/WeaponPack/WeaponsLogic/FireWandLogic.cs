using System.Collections;
using System.Collections.Generic;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using Other.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using Utils;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class FireWandLogic : ItemLogicBase
    {
        [SerializeField] private Color projectileLightColor;
        [SerializeField] private GameObject projectile;
        [SerializeField] private GameObject flightParticles;
        [SerializeField] private GameObject onHitParticles;

        private float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;
        
        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(this.projectile, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetTarget(target.transform)
                    .SetOnHitParticles(onHitParticles)
                    .SetOnHitAction(OnHitAction)
                    .SetFlightParticles(flightParticles)
                    .SetLightColor(projectileLightColor)
                    .SetEffect(EEffectType.Burn, EffectDuration)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private void OnHitAction(GameObject enemy, Projectile projectile)
        {
            var range = GetStatValue(EWeaponStat.BlastRange);
            var damage = GetStatValue(EWeaponStat.BlastDamage);

            if (range == null || damage == null) return;
            
            StartCoroutine(BoomCoroutine(enemy, range.Value, (int)damage.Value));
        }

        private IEnumerator BoomCoroutine(GameObject impactEnemy, float range, int damage)
        {
            var enemyInstanceId = impactEnemy.GetComponent<EnemyLogic>().GetInstanceID();
            var results = new Collider2D[50];
            var position = impactEnemy.transform.position;
            Physics2D.OverlapCircleNonAlloc(position, range, results);
            SpecialEffectManager.Instance
                .SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range);
            AudioManager.Instance.PlaySound(ESoundType.BananaBoom);
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