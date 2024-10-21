using System.Collections;
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
            StartCoroutine(ThrowShurikens());
            return UtilsMethods.FindNearestTarget(transform.position);
        }

        private IEnumerator ThrowShurikens()
        {
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position);
                if (target == null) continue;

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
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}