using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using Other.Enums;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class IceWandLogic : ItemLogicBase
    {
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private float animSpeed;
        [SerializeField] private GameObject flightParticles;

        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.EffectDuration,
            EItemSelfStatType.ProjectileScale
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
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = TargetManager.FindTarget(FindStrategy, 20f, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectileMovementStrategy = new TargetMovementStrategy(target.transform, Speed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetSprite(projectileSprites, animSpeed)
                    .SetEffect(EEffectType.Slow, EffectDuration)
                    .SetScale(Scale)
                    .Ready(); // TODO: flightParticles
                
                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }
    }
}