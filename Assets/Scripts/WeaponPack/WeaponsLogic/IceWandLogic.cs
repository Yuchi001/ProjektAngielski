using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class IceWandLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject iceProjectile;
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private float animSpeed;
        [SerializeField] private GameObject flightParticles;
        
        protected override void UseWeapon()
        {
            for (var i = 0; i < ProjectileCount; i++)
            {
                var projectile = Instantiate(iceProjectile, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetTarget(UtilsMethods.FindTarget(transform.position))
                    .SetSprite(projectileSprites, animSpeed)
                    .SetFlightParticles(flightParticles)
                    .SetReady();
            }
        }
    }
}