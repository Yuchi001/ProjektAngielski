using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other;
using TargetSearchPack;
using UnityEngine;
using TransformExtensions = Utils.TransformExtensions;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class SwordLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;

        private const string HitEnemyCountName = "HitCount";

        private bool _didComeBack = true;
        private float MaxRange => GetStatValue(EItemSelfStatType.ProjectileRange);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileRange,
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
            if (!_didComeBack) return false;
            
            var target = TargetManager.FindTarget(FindStrategy);
            if (target == null) return false;

            _didComeBack = false;
            var spawnedSword = Instantiate(Projectile, PlayerPos, Quaternion.identity);

            var enemyPos = target.transform.position;

            spawnedSword.Setup(Damage, Speed)
                .SetDirection(enemyPos)
                .SetLerp(enemyPos, 0.3f)
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

            spawnedSword.SetReady();

            return true;
        }

        private void OnHit(CanBeDamaged onHit, Projectile projectile)
        {
            if (onHit is not EnemyLogic enemyLogic) return;
            
            var count = projectile.GetCustomValue(HitEnemyCountName);
            count++;
            enemyLogic.GetDamaged(Damage * (int)count);
            projectile.SetCustomValue(HitEnemyCountName, count);
        }

        private void OnOutOfRange(Projectile projectile)
        {
            var newProjectile = Instantiate(Projectile, projectile.transform.position, Quaternion.identity);

            newProjectile.Setup(Damage, Speed)
                .SetTarget(PlayerTransform)
                .SetLerp(PlayerTransform, 0.3f)
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
            //projectile.SetSpeed(Speed + GetStatValue(EItemSelfStatType.ProjectileRange) * Time.deltaTime);
            var projectilePos = projectile.transform.position;
            var playerPos = PlayerTransform.position;

            var sr = projectile.GetSpriteRenderer().transform;
            TransformExtensions.LookAt(sr, PlayerPos);
            sr.Rotate(0, 0, 225);

            if (Vector2.Distance(projectilePos, playerPos) > 0.1f) return;

            _didComeBack = true;
            Destroy(projectile.gameObject);
        }

        private void ProjectileUpdate(Projectile projectile)
        {
            //projectile.SetSpeed(Speed + GetStatValue(EItemSelfStatType.ProjectileRange) * Time.deltaTime);
            var sr = projectile.GetSpriteRenderer().transform;
            TransformExtensions.LookAt(sr, PlayerPos);
            sr.Rotate(0, 0, 45 + 180);
        }
    }
}