using System.Collections.Generic;
using ItemPack.Enums;
using Other.Enums;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class IceWandLogic : ItemLogicBase
    {
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private float animSpeed;
        [SerializeField] private GameObject flightParticles;

        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration) ?? 0;
        
        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                
                projectile.Setup(Damage, Speed)
                    .SetTarget(target.transform)
                    .SetSprite(projectileSprites, animSpeed)
                    .SetFlightParticles(flightParticles)
                    .SetEffect(EEffectType.Slow, EffectDuration)
                    .SetReady();
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }
    }
}