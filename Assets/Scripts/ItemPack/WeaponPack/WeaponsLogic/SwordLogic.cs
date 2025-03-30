using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other.Enums;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class SwordLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;

        private const string HitEnemyCountName = "HitCount";

        private float MaxRange => GetStatValue(EItemSelfStatType.ProjectileRange);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileRange,
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        protected override bool Use()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindNearestTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;

                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

                var enemyPos = target.transform.position;

                projectile.Setup(Damage, Speed)
                    .SetDirection(enemyPos)
                    .SetSprite(projectileSprite)
                    .SetSpriteRotation(45)
                    .SetDontDestroyOnHit()
                    .SetUpdate(ProjectileUpdate)
                    .SetDisableDamageOnHit()
                    .SetNewCustomValue(HitEnemyCountName)
                    .SetOnHitAction(OnHit)
                    .SetScale(0.4f)
                    .SetRange(MaxRange)
                    .SetOutOfRangeBehaviour(OnOutOfRange);

                if (PlayerEnchantments.Has(EEnchantmentName.Sharpness))
                {
                    var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.Sharpness, EValueKey.Percentage);
                    if (Random.Range(0f, 1f) <= percentage) projectile.SetEffect(EEffectType.Bleed, 9999);
                }

                projectile.SetReady();

                targetedEnemies.Add(target.GetInstanceID());
            }

            return spawnedProjectiles > 0;
        }

        private void OnHit(GameObject onHit, Projectile projectile)
        {
            var count = projectile.GetCustomValue(HitEnemyCountName);
            count++;
            onHit.GetComponent<EnemyLogic>().GetDamaged(Damage * (int)count);
            projectile.SetCustomValue(HitEnemyCountName, count);
        }

        private void OnOutOfRange(Projectile projectile)
        {
            var newProjectile = Instantiate(Projectile, projectile.transform.position, Quaternion.identity);

            newProjectile.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetDirection(PlayerPos, 0, true)
                .SetSpriteRotation(225)
                .SetSprite(projectileSprite)
                .SetDontDestroyOnHit()
                .SetDisableDamageOnHit()
                .SetScale(0.4f)
                .SetUpdate(BackProjectileUpdate)
                .SetReady();

            Destroy(projectile.gameObject);
        }

        private void BackProjectileUpdate(Projectile projectile)
        {
            var projectilePos = projectile.transform.position;
            var playerPos = PlayerTransform.position;

            var sr = projectile.GetSpriteRenderer().transform;
            UtilsMethods.LookAtObj(sr, PlayerPos);
            sr.Rotate(0, 0, 225);

            if (Vector2.Distance(projectilePos, playerPos) > 0.5f) return;

            Destroy(projectile.gameObject);
        }

        private void ProjectileUpdate(Projectile projectile)
        {
            var sr = projectile.GetSpriteRenderer().transform;
            UtilsMethods.LookAtObj(sr, PlayerPos);
            sr.Rotate(0, 0, 45);
        }
    }
}