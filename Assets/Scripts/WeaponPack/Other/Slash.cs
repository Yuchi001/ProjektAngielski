using System;
using EnemyPack;
using Managers;
using Managers.Other;
using Other.Enums;
using PlayerPack;
using UnityEngine;

namespace WeaponPack.Other
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private Vector2 baseScale;
        [SerializeField] private float animTime = 0.2f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        private bool LookingRight => PlayerManager.Instance.PlayerMovement.LookingRight;
        
        private EEffectType? _effectType = null;

        private float _effectTime = -1;
        private float _pushForce = 3;
        
        private int _damage;
        
        #region Setup methods

        public Slash Setup(int damage, float scale)
        {
            _damage = damage;
            transform.localScale = baseScale * scale;
            return this;
        }

        public Slash SetEffect(EEffectType? effectType, float effectTime)
        {
            _effectType = effectType;
            _effectTime = effectTime;
            return this;
        }

        public Slash SetPushForce(float force)
        {
            _pushForce = force;
            return this;
        }

        public void Ready()
        {
            DamageEnemies();
        }

        #endregion

        private void DamageEnemies()
        {
            var spriteRender = spriteRenderer;
            var bounds = spriteRender.bounds;
            
            var newPos = transform.position;
            var mod = LookingRight ? 1 : -1;
            var offset = mod * bounds.size.x / 2;

            newPos.x += offset;
            transform.position = newPos;
            spriteRender.flipX = !LookingRight;
            
            var topRight = bounds.max;
            topRight.x += offset;
            var bottomLeft = bounds.min;
            bottomLeft.x += offset;
            
            var targets = Physics2D.OverlapAreaAll(topRight, bottomLeft);
            foreach (var target in targets)
            {
                if(!target.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.PushEnemy(Vector2.right * (mod * _pushForce), 0.3f);
                enemy.GetDamaged(_damage);
                if (!_effectType.HasValue) continue; 
                
                enemy.AddEffect(new EffectInfo
                {
                    effectType = _effectType.Value,
                    time = _effectTime,
                });
            }
            Destroy(gameObject, animTime);
        }
    }
}