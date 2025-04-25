using EnemyPack;
using Managers;
using Other.Enums;
using PlayerPack;
using ProjectilePack;
using TargetSearchPack;
using UnityEngine;
using Utils;

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
            var target = TargetManager.FindTarget(FindStrategy, range: baseScale.x);
            if (target == null)
            {
                Destroy(gameObject);
                return false;
            }
            
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

            foreach (var enemy in TargetDetector.EnemiesInSpriteBoundsArea(bottomLeft, topRight))
            {
                enemy.PushEnemy(PlayerPos, _pushForce);
                enemy.GetDamaged(_damage);
                if (!_effectType.HasValue) continue; 
                
                var effectContext = PlayerManager.GetEffectContextManager().GetEffectContext(_effectType.Value, _effectTime, enemy);
                enemy.AddEffect(effectContext);
            }
            Destroy(gameObject, animTime);
            return true;
        }
    }
}