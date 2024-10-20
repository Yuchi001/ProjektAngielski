﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Other.Enums;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class BookOfPoisonLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject fieldParticles;
        [SerializeField] private Sprite fieldSprite;
        [SerializeField] private float minimalFieldDistance = 0.4f;

        private float Duration => GetStatValue(EWeaponStat.Duration) ?? 0;
        private float Scale => GetStatValue(EWeaponStat.ProjectileScale) ?? 0;
        private float DamageRate => GetStatValue(EWeaponStat.DamageRate) ?? 1;
        private float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;

        private const string DamageRateName = "DamageRate";

        private readonly List<GameObject> _poisonFields = new();
        
        protected override bool UseWeapon()
        {
            _poisonFields.RemoveAll(go => go == null);

            if (_poisonFields.FirstOrDefault(go => IsTooClose(go.transform.position))) return false;
            
            var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(Damage, 0)
                .SetScale(Scale / 2f)
                .SetSprite(fieldSprite, Scale * 2)
                .SetLightColor(Color.clear)
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