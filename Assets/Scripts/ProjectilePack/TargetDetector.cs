using System;
using System.Collections.Generic;
using EnemyPack;
using Other;
using PlayerPack;
using PoolPack;
using UnityEngine;
using Utils;
using WorldGenerationPack;

namespace ProjectilePack
{
    public class TargetDetector
    {
        private readonly HashSet<CanBeDamaged> _currentTargets = new();
        private readonly HashSet<CanBeDamaged> _enemiesThisFrame = new();

        private Action<CanBeDamaged> _onTriggerEnter;
        private Action<CanBeDamaged> _onTriggerStay;

        private readonly Transform _monoTransform;
        private readonly float _range;
        private readonly string _targetTag;

        public TargetDetector(Transform monoTransform, float range, string targetTag)
        {
            _range = range;
            _monoTransform = monoTransform;
            _targetTag = targetTag;
        }

        public TargetDetector SetOnTriggerEnter(Action<CanBeDamaged> onTriggerEnter)
        {
            _onTriggerEnter = onTriggerEnter;
            return this;
        }

        public TargetDetector SetOnTriggerStay(Action<CanBeDamaged> onTriggerStay)
        {
            _onTriggerStay = onTriggerStay;
            return this;
        }

        public void CheckForTriggers()
        {
            _enemiesThisFrame.Clear();
            
            if (_targetTag == ProjectileManager.ENEMY_TAG) ManageEnemies();
            else if (_targetTag == ProjectileManager.PLAYER_TAG && PlayerManager.HasInstance()) ManageTarget(PlayerManager.PlayerHealth);

            _currentTargets?.RemoveWhere(enemy => !_enemiesThisFrame.Contains(enemy));
        }

        private void ManageTarget(CanBeDamaged target)
        {
            if (!IsInHitRange(target)) return;

            _enemiesThisFrame.Add(target);

            if (!_currentTargets.Contains(target))
            {
                _onTriggerEnter.Invoke(target);
                _currentTargets.Add(target);
            }
                
            _onTriggerStay?.Invoke(target);
        }

        private void ManageEnemies()
        {
            foreach (var hit in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (hit is not CanBeDamaged canBeDamaged) continue;
                ManageTarget(canBeDamaged);
            }
        }

        private bool IsInHitRange(CanBeDamaged canBeDamaged)
        {
            var transform = canBeDamaged.transform;
            var bulletRadius = _monoTransform.localScale.x * _range;
            var enemyRadius = canBeDamaged.BodyRadius * transform.localScale.x;
            var totalRange = 0.1f + bulletRadius + enemyRadius;
            var totalRangeSqr = totalRange * totalRange;

            return _monoTransform.InRange(transform.position, totalRangeSqr);
        }
        
        private static bool IsInHitRange(CanBeDamaged canBeDamaged, Vector3 center, float range)
        {
            var transform = canBeDamaged.transform;
            var enemyRadius = canBeDamaged.BodyRadius * transform.localScale.x;
            var totalRange = range + enemyRadius;
            var totalRangeSqr = totalRange * totalRange;

            return (transform.position - center).sqrMagnitude <= totalRangeSqr;
        }

        public static List<EnemyLogic> EnemiesInRange(Vector2 center, float range)
        {
            var enemiesInRange = new List<EnemyLogic>();
            foreach (var hit in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (hit is not EnemyLogic enemyLogic || !IsInHitRange(enemyLogic, center, range)) continue;
                enemiesInRange.Add(enemyLogic);
            }

            return enemiesInRange;
        }
    }
}