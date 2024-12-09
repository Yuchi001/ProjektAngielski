using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using PlayerPack;
using UnityEngine;

namespace EnemyPack.CustomEnemyLogic
{
    public partial class EnemyLogic
    {
        private static Projectile projectilePrefab => GameManager.Instance.GetPrefab<Projectile>(PrefabNames.Projectile);
        
        private void ShootOneBullet()
        {
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            projectile.Setup(bulletDamage, bulletSpeed)
                .SetSprite(projectileSprites, 5, 4)
                .SetTargetTag(playerTagName, 7)
                .SetDirection(PlayerManager.Instance.transform.position)
                .SetReady();
        }
    }
}