using System.Collections;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class GoldenSwordLogic : WeaponLogicBase
    {
        [SerializeField] private float animTime = 0.1f;
        [SerializeField] private GameObject lineRendererProjectilePrefab;

        private float Duration => GetStatValue(EWeaponStat.Duration) ?? 0;
        private float Scale => GetStatValue(EWeaponStat.ProjectileScale) ?? 1;
        private float DamageRate => GetStatValue(EWeaponStat.DamageRate) ?? 1;
        
        protected override bool UseWeapon()
        {
            var target = UtilsMethods.FindTarget();

            if (target == null) return false;
            
            var spawnedProjectile = Instantiate(lineRendererProjectilePrefab, transform.position, Quaternion.identity);
            var laserScript = spawnedProjectile.GetComponent<Laser>();
            
            laserScript.Setup(Damage, DamageRate)
                .SetTargetPosition(target.transform.position)
                .SetDuration(Duration)
                .SetAnimTime(animTime)
                .SetMaxScale(Scale)
                .Ready();

            return true;
        }
    }
}