using System.Collections.Generic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using UnityEngine;
using Utils;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BananaLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;

        private float Range => GetStatValue(EItemSelfStatType.ProjectileRange) ?? 2;
        
        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                
                targetedEnemies.Add(target.GetInstanceID());
                Projectile.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetOutOfRangeBehaviour(OutOfRangeBehaviour)
                    .SetSprite(projectileSprite)
                    .SetScale(0.5f)
                    .SetRange(Range)
                    .SetPushForce(PushForce)
                    .SetRotationSpeed(rotationSpeed)
                    .SetReady();
            }

            return spawnedProjectiles > 0;
        }

        private void OutOfRangeBehaviour(Projectile projectile)
        {
            var newProjectile = Instantiate(Projectile, projectile.transform.position, Quaternion.identity);
            
            newProjectile.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetSprite(projectileSprite)
                .SetDontDestroyOnHit()
                .SetScale(0.5f)
                .SetRotationSpeed(rotationSpeed)
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