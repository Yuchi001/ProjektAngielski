using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using Other.Enums;
using TargetSearchPack;
using UnityEngine;
using UnityEngine.Serialization;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class AxeLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float rotationSpeedModifier;

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
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

                projectile.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetSprite(projectileSprite)
                    .SetPushForce(PushForce)
                    .SetScale(ProjectileScale)
                    .SetRotationSpeed(-rotationSpeedModifier * Speed);
                
                projectile.SetReady();
            }

            return spawnedProjectiles > 0;
        }
    }
}