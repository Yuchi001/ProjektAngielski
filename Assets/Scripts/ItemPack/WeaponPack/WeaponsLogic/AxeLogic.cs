using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using ItemPack.Enums;
using Other.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

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

        protected override bool Use()
        {
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindTargetInBiggestGroup(transform.position);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

                projectile.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetSprite(projectileSprite)
                    .SetPushForce(PushForce)
                    .SetScale(ProjectileScale)
                    .SetRotationSpeed(-rotationSpeedModifier * Speed);
                
                if (PlayerEnchantments.Has(EEnchantmentName.Sharpness))
                {
                    var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.Sharpness, EValueKey.Percentage);
                    if (Random.Range(0f, 1f) <= percentage) projectile.SetEffect(EEffectType.Bleed, 9999);
                }
                
                projectile.SetReady();
            }

            return spawnedProjectiles > 0;
        }
    }
}