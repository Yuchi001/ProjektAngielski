using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BananaLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float rotationSpeed;

        private float Range => GetStatValue(EItemSelfStatType.ProjectileRange);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileRange
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStats);
        }

        private NearPlayerStrategy _nearPlayerStrategy;
        private NearPlayerStrategy NearPlayerStrategy
        {
            get
            {
                return _nearPlayerStrategy ??= new NearPlayerStrategy();
            }
        }

        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = TargetManager.FindTarget(NearPlayerStrategy, targetedEnemies); 
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectileMovementStrategy = new DirectionMovementStrategy(PlayerPos, 
                    target.transform.position, Speed, rotationSpeed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetDestroyOnCollision(false)
                    .SetOutOfRangeAction(OutOfRangeBehaviour)
                    .SetSprite(projectileSprite)
                    .SetScale(0.5f)
                    .SetRange(Range)
                    .SetPushForce(PushForce)
                    .Ready();
            }

            return spawnedProjectiles > 0;
        }

        private bool OutOfRangeBehaviour(Projectile projectile)
        {
            var projectileMovementStrategy = new TargetMovementStrategy(PlayerManager.GetTransform(), Speed, rotationSpeed);
            ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                .SetDestroyOnCollision(false)
                .SetUpdateAction(ProjectileUpdate)
                .SetSprite(projectileSprite)
                .SetScale(0.5f)
                .SetRange(9999)
                .Ready();
            
            return false;
        }

        private void ProjectileUpdate(Projectile projectile)
        {
            if (!projectile.transform.InRange(PlayerPos, 0.1f)) return;

            ProjectileManager.ReturnProjectile(projectile);
        }
    }
}