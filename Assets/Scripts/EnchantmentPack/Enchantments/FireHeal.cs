using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
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

        private void TriggerFireHeal(int value)
        {
            var position = PlayerManager.Instance.transform.position;
            var range = parameters[EValueKey.Range];
            var results = new Collider2D[50];
            
            AudioManager.Instance.PlaySound(ESoundType.BananaBoom);
            Physics2D.OverlapCircleNonAlloc(position, range, results);
            SpecialEffectManager.Instance.SpawnExplosion(ESpecialEffectType.ExplosionMedium,
                position, range).SetColor(Color.red);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.AddEffect(new EffectInfo
                {
                    effectType = EEffectType.Burn,
                    time = parameters[EValueKey.Time],
                });
                
                enemy.GetDamaged(Mathf.CeilToInt(value * parameters[EValueKey.Percentage]));
            }
        }
    }
}