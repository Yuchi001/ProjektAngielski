using System;
using EnchantmentPack.Enums;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using PlayerPack;
using PlayerPack.PlayerMovementPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class ExplosiveDash : EnchantmentBase
    {
        [SerializeField] private GameObject boomParticles;
        private static float enemyHpScale => GameManager.Instance.EnemySpawner.EnemiesHpScale;
        
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
            var playerPos = PlayerManager.Instance.transform.position;
            Physics2D.OverlapCircleNonAlloc(playerPos, parameters[EValueKey.Range], results);

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