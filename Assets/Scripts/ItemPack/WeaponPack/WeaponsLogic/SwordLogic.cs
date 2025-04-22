using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using TMPro;
using UnityEngine;
using Utils;
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

            var enemyPos = (Vector2)target.transform.position;
            var direction = (enemyPos - PlayerPos).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 45;
            var projectileMovementStrategy = new LerpMovementStrategy(PlayerPos, enemyPos, 1f / Speed);
            var spawnedSword = ProjectileManager.SpawnProjectile(projectileMovementStrategy, this);

            spawnedSword
                .SetSprite(projectileSprite, angle)
                .SetDestroyOnCollision(false)
                .SetUpdateAction(ProjectileUpdate)
                .SetAdditionalData(new SwordData())
                .SetOnHitAction(OnHit)
                .SetScale(0.4f)
                .SetRange(MaxRange)
                .SetOutOfRangeAction(OnOutOfRange);

            spawnedSword.Ready();

            return true;
        }

        private bool OnHit(Projectile projectile, CanBeDamaged hitObj)
        {
            var data = projectile.GetAdditionalData<SwordData>();
            data.IncrementEnemiesCount();
            projectile.SetAdditionalData(data);
            hitObj.GetDamaged(Damage * data.GetHitEnemiesCount());

            return true;
        }
        
        private bool OnOutOfRange(Projectile projectile)
        {
            var projectileMovementStrategy = new LerpToTargetMovementStrategy(projectile.transform.position,
                PlayerManager.GetTransform(), 1f / Speed);
            var newProjectile = ProjectileManager.SpawnProjectile(projectileMovementStrategy, this);

            newProjectile
                .SetSprite(projectileSprite)
                .SetDestroyOnCollision(false)
                .SetScale(0.4f)
                .SetOnHitAction(Projectile.CancelHit)
                .SetUpdateAction(BackProjectileUpdate)
                .Ready();

            return false;
        }

        private void BackProjectileUpdate(Projectile projectile)
        {
            var sr = projectile.SpriteRenderer.transform;
            TransformExtensions.LookAt(sr, PlayerPos);
            sr.Rotate(0, 0, 225);

            if (!PlayerTransform.InRange(projectile.transform.position, 0.1f)) return;

            _didComeBack = true;
            ProjectileManager.ReturnProjectile(projectile);
        }

        private void ProjectileUpdate(Projectile projectile)
        {
            var sr = projectile.SpriteRenderer.transform;
            TransformExtensions.LookAt(sr, PlayerPos);
            sr.Rotate(0, 0, 45 + 180);
        }

        private class SwordData
        {
            private int _hitEnemiesCount = 0;

            public int GetHitEnemiesCount() => _hitEnemiesCount;
            public void IncrementEnemiesCount() => _hitEnemiesCount++;
        }
    }
}