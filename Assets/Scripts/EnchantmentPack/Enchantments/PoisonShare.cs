using AudioPack;
using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Managers.Enums;
using Other.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
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

            var position = enemyLogic.transform.position;
            var range = parameters[EValueKey.Range];
            var results = new Collider2D[50];
            
            AudioManager.PlaySound(ESoundType.PoisonShare);
            Physics2D.OverlapCircleNonAlloc(position, range, results);
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range, Color.green);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.AddEffect(EEffectType.Poison, parameters[EValueKey.Time]);
            }
        }
    }
}