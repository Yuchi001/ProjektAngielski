using System.Collections.Generic;
using PlayerPack;
using UnityEngine;
using WeaponPack.Other;

namespace EnemyPack.CustomEnemyLogic
{
    public class DefaultBulletAction : EnemyAction
    {
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int damage;
        [SerializeField] private float speed;
        
        protected override void InvokeAction()
        {
            var projectile = Instantiate(projectilePrefab, EnemyLogic.transform.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(damage, speed)
                .SetSprite(projectileSprites, 5, 4)
                .SetLightColor(Color.red)
                .SetTargetTag(PlayerTagName, EnemyProjectileLayerMask)
                .SetDirection(PlayerManager.Instance.transform.position)
                .SetReady();
        }
    }
}