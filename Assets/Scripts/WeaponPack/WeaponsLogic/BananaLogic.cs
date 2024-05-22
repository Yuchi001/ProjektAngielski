using System.Collections.Generic;
using EnemyPack;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class BananaLogic : WeaponLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private GameObject projectilePrefab;

        private float Range => GetStatValue(EWeaponStat.ProjectileRange) ?? 2;
        
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
                
                targetedEnemies.Add(target.GetInstanceID());
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetOutOfRangeBehaviour(OutOfRangeBehaviour)
                    .SetSprite(projectileSprite)
                    .SetScale(0.5f)
                    .SetRange(Range)
                    .SetPushForce(PushForce)
                    .SetRotationSpeed(rotationSpeed)
                    .SetLightColor(Color.clear)
                    .SetReady();
            }
        }

        private void OutOfRangeBehaviour(Projectile projectile)
        {
            var newProjectile = Instantiate(projectilePrefab, projectile.transform.position, Quaternion.identity);
            var newProjectileScript = newProjectile.GetComponent<Projectile>();
            
            newProjectileScript.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetSprite(projectileSprite)
                .SetDontDestroyOnHit()
                .SetScale(0.5f)
                .SetRotationSpeed(rotationSpeed)
                .SetLightColor(Color.clear)
                .SetUpdate(ProjectileUpdate)
                .SetReady();
            
            Destroy(projectile.gameObject);
        }

        private void ProjectileUpdate(Projectile projectile)
        {
            var projectilePos = projectile.transform.position;
            var playerPos = PlayerTransform.position;
            if (Vector2.Distance(projectilePos, playerPos) > 0.1f) return;
            
            Destroy(projectile.gameObject);
        }
    }
}