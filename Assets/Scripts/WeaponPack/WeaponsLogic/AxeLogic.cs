using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class AxeLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;
        [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float rotationSpeedModifier;

        private float ProjectileScale => GetStatValue(EWeaponStat.ProjectileScale) ?? 0;
        
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
                    .SetSprite(projectileSprite)
                    .SetPushForce(PushForce)
                    .SetScale(ProjectileScale)
                    .SetRotationSpeed(-rotationSpeedModifier * Speed)
                    .SetLightColor(Color.clear)
                    .SetReady();
            }
        }
    }
}