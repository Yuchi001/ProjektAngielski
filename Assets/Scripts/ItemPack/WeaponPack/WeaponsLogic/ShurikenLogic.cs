using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other.Enums;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class ShurikenLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        protected override bool Use()
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

                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                
                projectile.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetSprite(projectileSprite)
                    .SetScale(0.3f)
                    .SetRotationSpeed(rotationSpeed)
                    .SetEffect(EEffectType.Bleed, 999)
                    .SetReady();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}