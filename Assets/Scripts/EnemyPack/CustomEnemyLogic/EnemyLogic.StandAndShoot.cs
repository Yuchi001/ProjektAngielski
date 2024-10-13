using System.Collections.Generic;
using PlayerPack;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponPack.Other;

namespace EnemyPack.CustomEnemyLogic
{
    public partial class EnemyLogic
    {
        [Header("Stand And Shoot")] 
        [SerializeField] private string playerTagName;
        [SerializeField] private LayerMask enemyProjectileLayerMask;
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int bulletDamage;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletsPerSec;
        
        private float _shootTimer = 0;
        
        private void UpdateStandAndShootBehaviour()
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer < 1f / bulletsPerSec) return;

            _shootTimer = 0;
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(bulletDamage, bulletSpeed)
                .SetSprite(projectileSprites, 5, 4)
                .SetLightColor(Color.red)
                .SetTargetTag(playerTagName, 7)
                .SetDirection(PlayerManager.Instance.transform.position)
                .SetReady();
        }
    }
}