using System;
using System.Collections.Generic;
using AccessorPack;
using Other;
using PoolPack;
using UnityEngine;
using Utils;

namespace ProjectilePack
{
    public class TriggerDetector
    {
        private readonly HashSet<PoolObject> _currentEnemies = new();
        private readonly HashSet<PoolObject> _enemiesThisFrame = new();

        private Action<CanBeDamaged> _onTriggerEnter;
        private Action<CanBeDamaged> _onTriggerStay;

        private readonly Transform _monoTransform;
        private readonly SpriteRenderer _monoSpriteRenderer;

        public TriggerDetector(SpriteRenderer spriteRenderer, Transform monoTransform = null)
        {
            _monoTransform = monoTransform ? monoTransform : spriteRenderer.transform;
            _monoSpriteRenderer = spriteRenderer;
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
            
            foreach (var hit in MainSceneAccessor.EnemySpawner.GetActiveEnemies())
            {
                if (hit == null) continue;
                
                if (hit is not CanBeDamaged canBeDamaged) continue; ;
                
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
            var bulletRadius = _monoSpriteRenderer.bounds.extents.magnitude;
            var enemyRadius = canBeDamaged.SpriteRenderer.bounds.extents.magnitude;
            var totalRange = 0.1f + bulletRadius + enemyRadius;
            var totalRangeSqr = totalRange * totalRange;

            return _monoTransform.InRange(canBeDamaged.transform.position, totalRangeSqr);
        }
    }
}