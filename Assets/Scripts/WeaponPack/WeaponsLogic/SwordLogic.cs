using System.Collections.Generic;
using EnemyPack;
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

        private float MaxEnemiesToHit => GetStatValue(EWeaponStat.MaxPiercedEnemies) ?? 0;
        
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
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetSprite(projectileSprite)
                    .SetSpriteRotation(45)
                    .SetDontDestroyOnHit()
                    .SetDisableDamageOnHit()
                    .SetNewCustomValue(HitEnemyCountName)
                    .SetOnHitAction(OnHit)
                    .SetScale(0.4f)
                    .SetLightColor(Color.clear)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private void OnHit(GameObject onHit, Projectile projectile)
        {
            var count = projectile.GetCustomValue(HitEnemyCountName);
            
            if (count >= MaxEnemiesToHit)
            {
                Destroy(projectile.gameObject);
                return;
            }

            count++;
            onHit.GetComponent<EnemyLogic>().GetDamaged(GetModifiedDamage((int)count));
            projectile.SetCustomValue(HitEnemyCountName, count);
        }

        private int GetModifiedDamage(int count)
        {
            var result = Damage;
            for (var i = 1; i < count; i++)
            {
                result *= 2;
            }
            return result;
        }
    }
}