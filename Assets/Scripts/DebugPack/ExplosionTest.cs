using System;
using System.Collections.Generic;
using EnemyPack;
using Other;
using ProjectilePack;
using UnityEngine;
using WorldGenerationPack;

namespace DebugPack
{
    public class ExplosionTest : MonoBehaviour
    {
        [SerializeField] private float explosionRange;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var enemy in EnemiesInRange(transform.position, explosionRange))
                {
                    Debug.Log(enemy.GetInstanceID());
                }
            }
        }
        
        public static List<CanBeDamaged> EnemiesInRange(Vector2 center, float range)
        {
            var enemiesInRange = new List<CanBeDamaged>();
            foreach (var hit in FindObjectsByType<CanBeDamaged>(FindObjectsSortMode.None))
            {
                if (!IsInHitRange(hit, center, range)) continue;
                enemiesInRange.Add(hit);
            }

            return enemiesInRange;
        }
        
        private static bool IsInHitRange(CanBeDamaged canBeDamaged, Vector3 center, float range)
        {
            var transform = canBeDamaged.transform;
            var enemyRadius = canBeDamaged.BodyRadius * transform.localScale.x;
            var totalRange = range + enemyRadius;
            var totalRangeSqr = totalRange * totalRange;

            return (transform.position - center).sqrMagnitude <= totalRangeSqr;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }
}