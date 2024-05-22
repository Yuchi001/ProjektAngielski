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
        
        protected override void UseWeapon()
        {
            var spawnedProjectile = Instantiate(lineRendererProjectilePrefab, transform.position, Quaternion.identity);
            var laserScript = spawnedProjectile.GetComponent<Laser>();
            
            var target = UtilsMethods.FindTarget();
            
            laserScript.Setup()
                .SetTarget(target)
                .SetDuration(Duration)
                .SetAnimTime(animTime)
                .SetMaxScale(Scale)
                .Ready();
        }
    }
}