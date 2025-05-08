using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using ItemPack.Enums;
using Other.Enums;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class ShurikenLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeedModifier;
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        private NearPlayerStrategy _findStrategy;
        private NearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new NearPlayerStrategy();
            }
        }

        protected override bool Use()
        {
            var target = FindTarget(FindStrategy, 20f);
            if (target == null) return false;
            
            StartCoroutine(ThrowShurikens(target));
            return true;
        }

        private IEnumerator ThrowShurikens(EnemyLogic target)
        {
            for (var i = 0; i < ProjectileCount; i++)
            {
                if (target == null) yield break;

                var projectileMovementStrategy = new DirectionMovementStrategy(PlayerPos, target.transform.position, Speed, rotationSpeedModifier * Speed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetSprite(projectileSprite)
                    .SetScale(0.3f)
                    .SetEffect(EEffectType.Bleed, 999)
                    .Ready();
                
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}