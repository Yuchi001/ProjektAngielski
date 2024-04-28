using System.Collections;
using System.Collections.Generic;
using EnemyPack;
using EnemyPack.SO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;
using EnemyLogic = EnemyPack.EnemyLogic;

namespace WeaponPack.WeaponsLogic
{
    public class FireWandLogic : WeaponLogicBase
    {
        [SerializeField] private Color projectileLightColor;
        [SerializeField] private GameObject projectile;
        [SerializeField] private GameObject flightParticles;
        [SerializeField] private GameObject onHitParticles;
        
        protected override void UseWeapon()
        {
            var targetedEnemies = new List<int>();
            for (var i = 0; i < ProjectileCount; i++)
            {
                var projectile = Instantiate(this.projectile, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();

                var target = UtilsMethods.FindTarget(transform.position, targetedEnemies);
                if (target == null)
                {
                    Destroy(projectile);
                    continue;
                }
                
                projectileScript.Setup(Damage, Speed)
                    .SetTarget(target.transform)
                    .SetOnHitParticles(onHitParticles)
                    .SetOnHitAction(OnHitAction)
                    .SetFlightParticles(flightParticles)
                    .SetLightColor(projectileLightColor)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }
        }

        private void OnHitAction(GameObject enemy)
        {
            var range = GetStatValue(EWeaponStat.BlastRange);
            var damage = GetStatValue(EWeaponStat.BlastDamage);

            if (range == null || damage == null) return;
            
            StartCoroutine(BoomCoroutine(enemy, range.Value, (int)damage.Value));
        }

        private IEnumerator BoomCoroutine(GameObject impactEnemy, float range, int damage)
        {
            var enemyInstanceId = impactEnemy.GetComponent<EnemyLogic>().GetInstanceID();
            var hitObjects = Physics2D.OverlapCircleAll(impactEnemy.transform.position, range);
            foreach (var hitCollider in hitObjects)
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