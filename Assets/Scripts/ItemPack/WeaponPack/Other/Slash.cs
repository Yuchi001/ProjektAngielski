using System.Collections.Generic;
using EnemyPack;
using Other.Enums;
using PlayerPack;
using ProjectilePack;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.Other
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private Vector2 baseScale;
        [SerializeField] private float animTime = 0.2f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private static Vector2 PlayerPos => PlayerManager.PlayerPos;
        
        private EEffectType? _effectType = null;

        private float _effectTime = -1;
        private float _pushForce = 3;
        
        private int _damage;

        private BiggestGroupNearPlayerStrategy _findStrategy;
        private BiggestGroupNearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new BiggestGroupNearPlayerStrategy(new NearPlayerStrategy());
            }
        }

        private List<EnemyLogic> _cachedTargetList = new();

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
        
        #endregion

        public bool DamageEnemies()
        {
            var spriteRender = spriteRenderer;
            var bounds = spriteRender.bounds;
            
            var newPos = transform.position;
            var found = TargetManager.TryFindViableTargets(FindStrategy, ref _cachedTargetList, baseScale.x);
            if (!found)
            {
                Destroy(gameObject);
                return false;
            }

            var target = FindStrategy.FindEnemy(_cachedTargetList);
            if (target == null)
            {
                Destroy(gameObject);
                return false;
            }
            
            _cachedTargetList.Clear();
            
            var lookingRight = target.transform.position.x > PlayerPos.x;
            var mod = lookingRight ? 1 : -1;
            var offset = mod * bounds.size.x / 2;

            newPos.x += offset;
            transform.position = newPos;
            spriteRender.flipX = !lookingRight;
            
            var topRight = bounds.max;
            topRight.x += offset;
            var bottomLeft = bounds.min;
            bottomLeft.x += offset;

            TargetDetector.EnemiesInSpriteBoundsArea(bottomLeft, topRight, ref _cachedTargetList);
            foreach (var enemy in _cachedTargetList)
            {
                enemy.PushEnemy(PlayerPos, _pushForce);
                enemy.GetDamaged(_damage);
                if (!_effectType.HasValue) continue; 
                
                var effectContext = PlayerManager.GetEffectContextManager().GetEffectContext(_effectType.Value, _effectTime, enemy);
                enemy.AddEffect(effectContext);
            }
            _cachedTargetList.Clear();
            Destroy(gameObject, animTime);
            return true;
        }
    }
}