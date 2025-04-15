using AudioPack;
using EnchantmentPack.Enums;
using EnemyPack;
using Managers;
using Managers.Enums;
using Other.Enums;
using PlayerPack;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class FireHeal : EnchantmentBase
    {
        private void Awake()
        {
            PlayerHealth.OnPlayerHeal += TriggerFireHeal;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerHeal -= TriggerFireHeal;
        }

        private void TriggerFireHeal(int value, int current)
        {
            var position = PlayerManager.PlayerPos;
            var range = parameters[EValueKey.Range];
            var results = new Collider2D[50];
            
            AudioManager.PlaySound(ESoundType.BananaBoom);
            Physics2D.OverlapCircleNonAlloc(position, range, results);
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range, Color.red);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.AddEffect(EEffectType.Burn, parameters[EValueKey.Time]);
                
                enemy.GetDamaged(Mathf.CeilToInt(value * parameters[EValueKey.Percentage]));
            }
        }
    }
}