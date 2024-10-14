using System;
using System.Collections.Generic;
using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Other.Enums;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class PoisonShare : EnchantmentBase
    {
        private void Awake()
        {
            EnemySpawner.OnEnemyDie += TriggerPoisonSpread;
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemyDie -= TriggerPoisonSpread;
        }

        private void TriggerPoisonSpread(EnemyLogic enemyLogic)
        {
            if (!enemyLogic.HasEffect(EEffectType.Poison)) return;
            
            var results = new Collider2D[50];
            Physics2D.OverlapCircleNonAlloc(PlayerManager.Instance.transform.position, parameters[EValueKey.Range], results);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.AddEffect(new EffectInfo
                {
                    effectType = EEffectType.Poison,
                    time = parameters[EValueKey.Time],
                });
            }
        }
    }
}