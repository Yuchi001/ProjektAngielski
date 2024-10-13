using Other.Enums;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class ShurikenLogic : WeaponLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private GameObject projectilePrefab;
        protected override bool UseWeapon()
        {
            var spawned = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindTarget(transform.position);
                if (target == null) continue;

                spawned++;
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetSprite(projectileSprite)
                    .SetScale(0.3f)
                    .SetRotationSpeed(rotationSpeed)
                    .SetLightColor(Color.clear)
                    .SetEffect(EEffectType.Bleed, 999)
                    .SetReady();
            }

            return spawned > 0;
        }
    }
}