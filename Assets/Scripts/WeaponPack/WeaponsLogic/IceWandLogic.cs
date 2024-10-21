using System.Collections.Generic;
using Other.Enums;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class IceWandLogic : WeaponLogicBase
    {
        [SerializeField] private Color projectileLightColor;
        [SerializeField] private GameObject iceProjectile;
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private float animSpeed;
        [SerializeField] private GameObject flightParticles;

        private float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;
        
        protected override bool UseWeapon()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(iceProjectile, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetTarget(target.transform)
                    .SetSprite(projectileSprites, animSpeed)
                    .SetFlightParticles(flightParticles)
                    .SetLightColor(projectileLightColor)
                    .SetEffect(EEffectType.Slow, EffectDuration)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }
    }
}