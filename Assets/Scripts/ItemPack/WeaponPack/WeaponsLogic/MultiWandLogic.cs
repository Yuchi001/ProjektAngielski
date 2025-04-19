using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class MultiWandLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileScale
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        protected override bool Use()
        {
            StartCoroutine(SpawnParticles());

            return true;
        }

        private IEnumerator SpawnParticles()
        {
            var alpha = 90f / ProjectileCount;

            var currentAngle = alpha;
            
            for (var i = 0; i < ProjectileCount; i++)
            {
                var position = PlayerTransform.position;
                var up = PlayerTransform.up;
                var right = PlayerTransform.right;
            
                const int rotCount = 4;
            
                var rotations = new List<Vector2>(rotCount)
                {
                    up + position,
                    -up + position,
                    right + position,
                    -right + position,
                };

                
                for (var j = 0; j < rotCount; j++)
                {
                    var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                    
                    projectile.Setup(Damage, Speed)
                        .SetSprite(projectileSprite)
                        .SetDirection(rotations[j], currentAngle)
                        .SetReady();
                }
                
                currentAngle += alpha;
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}