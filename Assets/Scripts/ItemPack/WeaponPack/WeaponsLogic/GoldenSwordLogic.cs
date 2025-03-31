using System.Collections.Generic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class GoldenSwordLogic : ItemLogicBase
    {
        [SerializeField] private float animTime = 0.1f;

        private Laser laserProjectile;
        private Laser LaserProjectile
        {
            get
            {
                if (laserProjectile == null) laserProjectile =GameManager.GetPrefab<Laser>(PrefabNames.LaserProjectile);
                return laserProjectile;
            }
        }

        private float Duration => GetStatValue(EItemSelfStatType.Duration);
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale);
        private float DamageRate => GetStatValue(EItemSelfStatType.DamageRate);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.Duration,
            EItemSelfStatType.ProjectileScale,
            EItemSelfStatType.DamageRate,
        };

        protected override bool Use()
        {
            var target = UtilsMethods.FindFurthestTarget(PlayerPos);

            if (target == null) return false;
            
            var spawnedProjectile = Instantiate(LaserProjectile, transform.position, Quaternion.identity);
            
            spawnedProjectile.Setup(Damage, DamageRate)
                .SetTargetPosition(target.transform.position)
                .SetDuration(Duration)
                .SetAnimTime(animTime)
                .SetMaxScale(Scale)
                .Ready();

            return true;
        }
    }
}