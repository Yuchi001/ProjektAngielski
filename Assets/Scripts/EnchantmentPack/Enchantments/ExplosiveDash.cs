using AccessorPack;
using AudioPack;
using DifficultyPack;
using EnchantmentPack.Enums;
using EnemyPack;
using Managers;
using Managers.Enums;
using PlayerPack;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class ExplosiveDash : EnchantmentBase
    {
        [SerializeField] private GameObject boomParticles;
        private static float enemyHpScale => DifficultyManager.EnemyHpScale;
        
        private void Awake()
        {
            PlayerMovement.OnPlayerDashEnd += TriggerExplosion;
        }

        private void OnDisable()
        {
            PlayerMovement.OnPlayerDashEnd -= TriggerExplosion;
        }

        private void TriggerExplosion()
        {
            var results = new Collider2D[50];
            var playerPos = PlayerManager.PlayerPos;
            var range = parameters[EValueKey.Range];
            Physics2D.OverlapCircleNonAlloc(playerPos, range, results);

            AudioManager.PlaySound(ESoundType.BananaBoom);
            SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium, playerPos, range);

            var particles = Instantiate(boomParticles, playerPos, Quaternion.identity);
            Destroy(particles, 2f);
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged((int)(enemyHpScale * parameters[EValueKey.Damage]));
            }
        }
    }
}