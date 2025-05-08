using System;
using System.Collections.Generic;
using EnemyPack;
using Other;
using PlayerPack;
using UnityEngine;
using Utils;

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

        private List<EnemyLogic> _cachedTargetList = new();

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

            _currentTargets?.ExceptWith(_enemiesThisFrame);
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
            var success = EnemyManager.GetNearbyEnemies(_monoTransform.position, _range, ref _cachedTargetList);
            if (!success) return;
            
            foreach (var enemy in _cachedTargetList) ManageTarget(enemy);
            _cachedTargetList.Clear();
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

        public static bool TryGetEnemiesInRange(Vector2 center, float range, ref List<EnemyLogic> foundTargets)
        {
            if (!EnemyManager.GetNearbyEnemies(center, range, ref foundTargets)) return false;

            for (var i = foundTargets.Count - 1; i >= 0; i--)
            {
                if (IsInHitRange(foundTargets[i], center, range)) continue;
                foundTargets.RemoveAt(i);
            }

            return foundTargets.Count > 0;
        }
        
        public static bool EnemiesInSpriteBoundsArea(Vector2 bottomLeft, Vector2 topRight, ref List<EnemyLogic> candidates)
        {
            var center = (bottomLeft + topRight) * 0.5f;
            var halfWidth = (topRight.x - bottomLeft.x) * 0.5f;
            var halfHeight = (topRight.y - bottomLeft.y) * 0.5f;
            var maxDistanceSqr = halfWidth * halfWidth + halfHeight * halfHeight;

            if (!EnemyManager.GetNearbyEnemies(center, Mathf.Sqrt(maxDistanceSqr), ref candidates)) return false;

            foreach (var enemy in candidates)
            {
                var pos = (Vector2)enemy.transform.position;

                if (pos.x >= bottomLeft.x && pos.x <= topRight.x &&
                    pos.y >= bottomLeft.y && pos.y <= topRight.y)
                {
                    candidates.Add(enemy);
                }
            }

            return candidates.Count > 0;
        }
    }
}