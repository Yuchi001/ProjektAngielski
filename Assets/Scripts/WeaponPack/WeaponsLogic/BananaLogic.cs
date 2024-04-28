using System.Collections.Generic;
using EnemyPack;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class BananaLogic : WeaponLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float range;
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
                    .SetRange(range)
                    .SetRotationSpeed(rotationSpeed)
                    .SetLightColor(Color.clear)
                    .SetReady();
            }
        }

        private void OutOfRangeBehaviour(GameObject thisGameObject)
        {
            var projectile = Instantiate(projectilePrefab, thisGameObject.transform.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();
           
            projectileScript.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetSprite(projectileSprite)
                .SetDontDestroyOnHit()
                .SetScale(0.5f)
                .SetRotationSpeed(rotationSpeed)
                .SetLightColor(Color.clear)
                .SetReady();
            
            Destroy(thisGameObject);
        }
    }
}