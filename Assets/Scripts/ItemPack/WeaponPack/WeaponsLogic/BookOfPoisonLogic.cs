using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other.Enums;
using UnityEngine;

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

        private const string DamageRateName = "DamageRate";

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
            
            var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

            projectile.Setup(Damage, 0)
                .SetScale(Scale / 2f)
                .SetSprite(fieldSprite, Scale * 2)
                .SetLifeTime(Duration)
                .SetDontDestroyOnHit()
                .SetNewCustomValue(DamageRateName)
                .SetSortingLayer("Floor", 1)
                .SetUpdate(ParticleUpdate)
                .SetDisableDestroyOnContactWithWall()
                .SetOnCollisionStay(CollisionStay)
                .SetEffect(EEffectType.Poison, EffectDuration)
                .SetFlightParticles(fieldParticles, Scale * 2, true)
                .SetReady();
            
            _poisonFields.Add(projectile);

            return true;

            bool IsTooClose(Vector2 pos) => Vector2.Distance(pos, PlayerPos) < minimalFieldDistance;
        }

        private void ParticleUpdate(Projectile projectile)
        {
            var currentRate = projectile.GetCustomValue(DamageRateName);
            projectile.SetCustomValue(DamageRateName, currentRate + Time.deltaTime);
        }

        private void CollisionStay(GameObject enemy, Projectile projectile)
        {
            if (projectile.GetCustomValue(DamageRateName) < 1f / DamageRate) return;
            
            var enemyScript = enemy.GetComponent<EnemyLogic>();
            enemyScript.GetDamaged(Damage);
            projectile.SetCustomValue(DamageRateName, 0);
        }
        
        protected override float CustomCooldownModifier()
        {
            var stacks = PlayerEnchantments.GetStacks(EEnchantmentName.BetterBooks);
            if (stacks <= 0) return base.CustomCooldownModifier();
            var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.BetterBooks, EValueKey.Percentage);
            return 1 + percentage * stacks;
        }
    }
}