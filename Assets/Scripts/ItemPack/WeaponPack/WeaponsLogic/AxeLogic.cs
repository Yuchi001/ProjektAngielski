using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class AxeLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeedModifier;

        private float ProjectileScale => GetStatValue(EItemSelfStatType.ProjectileScale);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileScale,
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStats);
        }

        private BiggestGroupNearPlayerStrategy _findStrategy;
        private BiggestGroupNearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new BiggestGroupNearPlayerStrategy(new NearPlayerStrategy());
            }
        }
        
        protected override bool Use()
        {
            var spawnedProjectiles = 0;
            var targetedEnemies = new List<int>();
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = TargetManager.FindTarget(FindStrategy, targetedEnemies);
                if (target == null) continue;

                targetedEnemies.Add(target.GetInstanceID());
                spawnedProjectiles++;

                var projectileMovementStrategy = new DirectionMovementStrategy(PlayerPos, 
                    target.transform.position, Speed, -rotationSpeedModifier * Speed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetDestroyOnCollision(false)
                    .SetSprite(projectileSprite)
                    .SetScale(ProjectileScale)
                    .SetPushForce(PushForce)
                    .Ready();
            }

            return spawnedProjectiles > 0;
        }
    }
}