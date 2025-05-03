using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using ItemPack.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfPoisonLogic : ItemLogicBase
    {
        [SerializeField] private GameObject fieldParticles;
        [SerializeField] private Sprite fieldSprite;
        [SerializeField] private float minimalFieldDistance = 0.4f;

        private float Duration => GetStatValue(EItemSelfStatType.Duration);
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale);
        private float DamageRate => GetStatValue(EItemSelfStatType.DamageRate);
        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);

        private const string BOOK_OF_POISON_TIMER_ID = "BOOK_OF_POISON_TIMER_ID";

        private readonly List<Projectile> _poisonFields = new();

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.Duration,
            EItemSelfStatType.ProjectileScale,
            EItemSelfStatType.DamageRate,
            EItemSelfStatType.EffectDuration
        };
        
        protected override bool Use()
        {
            _poisonFields.RemoveAll(go => go == null);

            if (_poisonFields.FirstOrDefault(go => IsTooClose(go.transform.position))) return false;
            
            var spawnedPoisonFields = ProjectileManager.SpawnProjectile(IProjectileMovementStrategy.IGNORE, this)
                .SetScale(Scale)
                .SetSprite(fieldSprite)
                .SetLifeTime(Duration)
                .SetDestroyOnCollision(false)
                .SetSortingLayer("Floor", 1)
                .SetOnHitStayAction(HitStayAction)
                .SetEffect(EEffectType.Poison, EffectDuration)
                .Ready(); //TODO PARTICLES
            
            spawnedPoisonFields.SetTimer(BOOK_OF_POISON_TIMER_ID);
            
            _poisonFields.Add(spawnedPoisonFields);

            return true;

            bool IsTooClose(Vector2 pos) => PlayerManager.GetTransform().InRange(pos, minimalFieldDistance);
        }

        private void HitStayAction(Projectile projectile, CanBeDamaged enemy)
        {
            if (projectile.CheckTimer(BOOK_OF_POISON_TIMER_ID) < 1f / DamageRate) return;
            projectile.SetTimer(BOOK_OF_POISON_TIMER_ID);

            if (enemy is not EnemyLogic enemyScript) return;
            
            enemyScript.GetDamaged(Damage);
        }
    }
}