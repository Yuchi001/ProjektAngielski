using System.Collections.Generic;
using UnityEngine;
using Utils;
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
        
        protected override void UseWeapon()
        {
            var targetedEnemies = new List<int>();
            for (var i = 0; i < ProjectileCount; i++)
            {
                var projectile = Instantiate(iceProjectile, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();

                var target = UtilsMethods.FindTarget(transform.position, targetedEnemies);
                if (target == null)
                {
                    Destroy(projectile);
                    continue;
                }
                
                projectileScript.Setup(Damage, Speed)
                    .SetTarget(target)
                    .SetSprite(projectileSprites, animSpeed)
                    .SetFlightParticles(flightParticles)
                    .SetLightColor(projectileLightColor)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }
        }
    }
}