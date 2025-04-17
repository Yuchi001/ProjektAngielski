using System.Collections.Generic;
using System.Linq;
using AudioPack;
using EnemyPack;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers.Enums;
using Other;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic.Base
{
    public abstract class FireArmBase : ItemLogicBase
    {
        [SerializeField] protected float bulletScale = 0.2f;
        [SerializeField] protected float trailTime = 0.2f;
        [SerializeField] protected Sprite projectileSprite;
        private float Spread => GetStatValue(EItemSelfStatType.Spread);
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.Spread
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStats);
        }

        
        /// <summary>
        /// Base for fire arm bullet, options already set:
        ///<br/> - scale
        ///<br/> - sprite
        ///<br/> - push force
        ///<br/> - trail
        ///<br/> - on hit action
        /// </summary>
        /// <returns></returns>
        protected Projectile SpawnProjectile(Vector2 position)
        {
            var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

            projectile.Setup(Damage, Speed)
                .SetDirection(GetDirection(position))
                .SetSprite(projectileSprite)
                .SetScale(bulletScale)
                .SetTrail(trailTime)
                .SetPushForce(PushForce);

            return projectile;
        }
        
        private Vector2 GetDirection(Vector2 pickedTargetPos)
        {
            var pos = pickedTargetPos;
            pos.x += Random.Range(-Spread, Spread);
            pos.y += Random.Range(-Spread, Spread);
            return pos;
        }
    }
}