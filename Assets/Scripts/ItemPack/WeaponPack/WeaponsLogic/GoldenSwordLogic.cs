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

        private static Laser LaserProjectile => GameManager.Instance.GetPrefab<Laser>(PrefabNames.LaserProjectile);

        private float Duration => GetStatValue(EItemSelfStatType.Duration) ?? 0;
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale) ?? 1;
        private float DamageRate => GetStatValue(EItemSelfStatType.DamageRate) ?? 1;
        
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