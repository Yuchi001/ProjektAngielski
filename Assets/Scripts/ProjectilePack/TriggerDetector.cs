using System;
using System.Collections.Generic;
using Other;
using PoolPack;
using UnityEngine;
using Utils;
using WorldGenerationPack;

namespace ProjectilePack
{
    public class TriggerDetector
    {
        private readonly HashSet<PoolObject> _currentEnemies = new();
        private readonly HashSet<PoolObject> _enemiesThisFrame = new();

        private Action<CanBeDamaged> _onTriggerEnter;
        private Action<CanBeDamaged> _onTriggerStay;

        private readonly Transform _monoTransform;
        private readonly float _range;
        private readonly string _targetTag;

        public TriggerDetector(Transform monoTransform, float range, string targetTag)
        {
            _range = range;
            _monoTransform = monoTransform;
            _targetTag = targetTag;
        }

        public TriggerDetector SetOnTriggerEnter(Action<CanBeDamaged> onTriggerEnter)
        {
            _onTriggerEnter = onTriggerEnter;
            return this;
        }

        public TriggerDetector SetOnTriggerStay(Action<CanBeDamaged> onTriggerStay)
        {
            _onTriggerStay = onTriggerStay;
            return this;
        }

        public void CheckForTriggers()
        {
            _enemiesThisFrame.Clear();
            
            foreach (var hit in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (hit == null) continue;
                
                if (hit is not CanBeDamaged canBeDamaged) continue;
                
                if (!hit.CompareTag(_targetTag)) continue;
                
                if (!IsInHitRange(canBeDamaged)) continue;

                _enemiesThisFrame.Add(canBeDamaged);

                if (!_currentEnemies.Contains(canBeDamaged))
                {
                    _onTriggerEnter.Invoke(canBeDamaged);
                    _currentEnemies.Add(canBeDamaged);
                }


                _onTriggerStay?.Invoke(canBeDamaged);
            }

            _currentEnemies?.RemoveWhere(enemy => !_enemiesThisFrame.Contains(enemy));
        }

        private bool IsInHitRange(CanBeDamaged canBeDamaged)
        {
            var bulletRadius = _monoTransform.localScale.x * _range;
            var enemyRadius = canBeDamaged.BodyRadius * canBeDamaged.transform.localScale.x;
            var totalRange = 0.1f + bulletRadius + enemyRadius;
            var totalRangeSqr = totalRange * totalRange;

            return _monoTransform.InRange(canBeDamaged.transform.position, totalRangeSqr);
        }
        
    }
}