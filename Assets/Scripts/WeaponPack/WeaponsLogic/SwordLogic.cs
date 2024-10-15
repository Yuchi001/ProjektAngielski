using System.Collections.Generic;
using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Other.Enums;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class SwordLogic : WeaponLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject projectilePrefab;

        private const string HitEnemyCountName = "HitCount";

        private float MaxRange => GetStatValue(EWeaponStat.ProjectileRange) ?? 0;
        
        protected override bool UseWeapon()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();

                var enemyPos = target.transform.position;

                projectileScript.Setup(Damage, Speed)
                    .SetDirection(enemyPos)
                    .SetSprite(projectileSprite)
                    .SetSpriteRotation(45)
                    .SetDontDestroyOnHit()
                    .SetUpdate(ProjectileUpdate)
                    .SetDisableDamageOnHit()
                    .SetNewCustomValue(HitEnemyCountName)
                    .SetOnHitAction(OnHit)
                    .SetScale(0.4f)
                    .SetRange(MaxRange)
                    .SetLightColor(Color.clear)
                    .SetOutOfRangeBehaviour(OnOutOfRange);
                
                if (PlayerEnchantmentManager.Has(EEnchantmentName.Sharpness))
                {
                    var parameters = GameManager.Instance.EnchantmentValueDictionary[EEnchantmentName.Sharpness];
                    if (Random.Range(0f, 1f) <= parameters[EValueKey.Percentage]) projectileScript.SetEffect(EEffectType.Bleed, 9999);
                }
                
                projectileScript.SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private void OnHit(GameObject onHit, Projectile projectile)
        {
            var count = projectile.GetCustomValue(HitEnemyCountName);
            count++;
            onHit.GetComponent<EnemyLogic>().GetDamaged(Damage * (int)count);
            projectile.SetCustomValue(HitEnemyCountName, count);
        }

        private void OnOutOfRange(Projectile projectile)
        {
            var newProjectile = Instantiate(projectilePrefab, projectile.transform.position, Quaternion.identity);
            var newProjectileScript = newProjectile.GetComponent<Projectile>();
            
            newProjectileScript.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetDirection(PlayerPos, 0, true)
                .SetSpriteRotation(225)
                .SetSprite(projectileSprite)
                .SetDontDestroyOnHit()
                .SetDisableDamageOnHit()
                .SetScale(0.4f)
                .SetLightColor(Color.clear)
                .SetUpdate(BackProjectileUpdate)
                .SetReady();
            
            Destroy(projectile.gameObject);
        }
        
        private void BackProjectileUpdate(Projectile projectile)
        {
            var projectilePos = projectile.transform.position;
            var playerPos = PlayerTransform.position;
            
            var sr = projectile.GetSpriteRenderer().transform;
            UtilsMethods.LookAtObj(sr, PlayerPos);
            sr.Rotate(0,0, 225);
            
            if (Vector2.Distance(projectilePos, playerPos) > 0.5f) return;
            
            Destroy(projectile.gameObject);
        }
        
        private void ProjectileUpdate(Projectile projectile)
        {
            var sr = projectile.GetSpriteRenderer().transform;
            UtilsMethods.LookAtObj(sr, PlayerPos);
            sr.Rotate(0,0, 45);
        }
    }
}