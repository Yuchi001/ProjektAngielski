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
        
        protected override void UseWeapon()
        {
            var targetedEnemies = new List<int>();
            for (var i = 0; i < ProjectileCount; i++)
            {
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();

                var target = UtilsMethods.FindTarget(transform.position, targetedEnemies);
                if (target == null)
                {
                    Destroy(projectile);
                    continue;
                }
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetSprite(projectileSprite)
                    .SetSpriteRotation(45)
                    .SetDontDestroyOnHit()
                    .SetDisableDamageOnHit()
                    .SetCustomValue(0, HitEnemyCountName)
                    .SetOnHitAction(OnHit)
                    .SetScale(0.4f)
                    .SetLightColor(Color.clear)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }
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