using System;
using System.Collections.Generic;
using EnemyPack;
using ItemPack.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using PoolPack;
using UnityEngine;
using Utils;

namespace ProjectilePack
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : PoolObject
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private SpriteRenderer projectileSpriteRenderer;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float bulletRange;

        // GETTERS
        public SpriteRenderer SpriteRenderer => projectileSpriteRenderer;

        // RESET VALUES
        private float RESET_bulletRange;
        private string RESET_sortingLayer;
        private int RESET_sortingOrder;
        private float RESET_maxDistance;

        // SET VALUES
        private Vector2 SET_startPosition;
        
        // UPDATE VALUES
        private float UPDATE_currentLifeTime;
        private float UPDATE_animationTimer;

        private IProjectileMovementStrategy _projectileMovementStrategy;
        private TargetDetector _targetDetector;

        private ProjectileManager _poolManager;

        // GENERAL
        private int _damage;
        private string _targetTag;
        private bool _destroyOnCollision;
        private float? _pushForce;
        private float? _lifeTime;
        private List<EItemTag> _itemTags;
        
        // ANIMATION
        private CircularEnumerable<Sprite> _animationSprites;
        private float _animationSpeed;
        
        // ADDITIONAL ACTIONS
        private Func<Projectile, CanBeDamaged, bool> _onHitAction;
        private Action<Projectile, CanBeDamaged> _onHitStayAction;
        private Action<Projectile> _onUpdateAction;
        private Func<Projectile, bool> _onOutOfRangeAction; // returns info if we should brake from base behaviour

        // CUSTOM DATA
        private object _additionalData;
        
        // EFFECT DATA
        private EEffectType _effectType;
        private float _effectTime;

        public Projectile Setup(IProjectileMovementStrategy projectileMovementStrategy, int damage, string targetTag)
        {
            _projectileMovementStrategy = projectileMovementStrategy;
            _damage = damage;
            _targetTag = targetTag;
            return this;
        }
        
        public Projectile Setup(IProjectileMovementStrategy projectileMovementStrategy, int damage, string targetTag, List<EItemTag> itemTags)
        {
            OnGet(null);
            _projectileMovementStrategy = projectileMovementStrategy;
            _damage = damage;
            _targetTag = targetTag;
            _itemTags = itemTags;
            return this;
        }

        public Projectile SetRange(float range)
        {
            maxDistance = range;
            return this;
        }

        public Projectile SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale);
            return this;
        }

        public Projectile SetTrail(float trailTime)
        {
            trailRenderer.enabled = true;
            trailRenderer.time = trailTime;
            return this;
        }

        public Projectile SetOutOfRangeAction(Func<Projectile, bool> outOfRangeAction)
        {
            _onOutOfRangeAction = outOfRangeAction;
            return this;
        }

        public Projectile SetDestroyOnCollision(bool destroyOnCollision)
        {
            _destroyOnCollision = destroyOnCollision;
            return this;
        }

        public Projectile SetPushForce(float pushForce)
        {
            _pushForce = pushForce;
            return this;
        }

        public Projectile SetOnHitAction(Func<Projectile, CanBeDamaged, bool> onHitAction)
        {
            _onHitAction = onHitAction;
            return this;
        }

        public Projectile SetLifeTime(float lifeTime)
        {
            _lifeTime = lifeTime;
            return this;
        }
        
        public Projectile SetSprite(Sprite sprite, float spriteAngle = 0)
        {
            projectileSpriteRenderer.sprite = sprite;
            projectileSpriteRenderer.enabled = true;
            projectileSpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, spriteAngle);
            return this;
        }

        public Projectile SetFlip(bool flipX = false, bool flipY = false)
        {
            projectileSpriteRenderer.flipX = flipX;
            projectileSpriteRenderer.flipY = flipY;
            return this;
        }

        public Projectile SetSprite(List<Sprite> sprites, float animSpeed, float spriteAngle = 0)
        {
            _animationSprites = new CircularEnumerable<Sprite>(sprites);
            _animationSpeed = animSpeed;
            return SetSprite(sprites[0], spriteAngle);
        }
        
        public Projectile SetOnHitStayAction(Action<Projectile, CanBeDamaged> onHitStayAction)
        {
            _onHitStayAction = onHitStayAction;
            return this;
        }

        public Projectile SetSortingLayer(string layerName, int order)
        {
            projectileSpriteRenderer.sortingOrder = order;
            projectileSpriteRenderer.sortingLayerName = layerName;
            return this;
        }

        public Projectile SetUpdateAction(Action<Projectile> onUpdateAction)
        {
            _onUpdateAction = onUpdateAction;
            return this;
        }

        public Projectile SetEffect(EEffectType effectType, float effectTime)
        {
            _effectType = effectType;
            _effectTime = effectTime;
            return this;
        }

        public Projectile Ready()
        {
            _targetDetector = new TargetDetector(transform, bulletRange, _targetTag);
            _targetDetector.SetOnTriggerEnter(OnTargetHit);
            _targetDetector.SetOnTriggerStay(OnTargetHitStay);
            SET_startPosition = transform.position;
            UPDATE_currentLifeTime = 0;
            UPDATE_animationTimer = 0;
            return this;
        }

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            RESET_bulletRange = bulletRange;
            RESET_maxDistance = maxDistance;
            RESET_sortingOrder = projectileSpriteRenderer.sortingOrder;
            RESET_sortingLayer = projectileSpriteRenderer.sortingLayerName;
            _poolManager = (ProjectileManager)poolManager;
        }

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);
            _damage = 0;
            _itemTags = new List<EItemTag>();
            _pushForce = null;
            _lifeTime = null;
            _animationSprites = new CircularEnumerable<Sprite>();
            _destroyOnCollision = true;
            transform.localScale = Vector3.one;
            trailRenderer.enabled = false;
            maxDistance = RESET_maxDistance;
            bulletRange = RESET_bulletRange;
            
            projectileSpriteRenderer.enabled = false;
            projectileSpriteRenderer.sortingLayerName = RESET_sortingLayer;
            projectileSpriteRenderer.sortingOrder = RESET_sortingOrder;
            projectileSpriteRenderer.transform.rotation = Quaternion.identity;
            projectileSpriteRenderer.flipX = false;
            projectileSpriteRenderer.flipY = false;
            
            _effectType = EEffectType.None;
            _effectTime = 0;
            _targetTag = "";
        }

        public override void OnRelease()
        {
            base.OnRelease();
            _targetDetector = null;
            _onHitAction = null;
            _onHitStayAction = null;
            _onUpdateAction = null;
            _additionalData = null;
            _onOutOfRangeAction = null;
            trailRenderer.Clear();
        }

        public override void InvokeUpdate()
        {
            if (!Active) return;
            
            base.InvokeUpdate();
            ManageLifeTime();
            ManageAnimation();
            _targetDetector?.CheckForTriggers();
            
            _onUpdateAction?.Invoke(this);
            _projectileMovementStrategy.MoveProjectile(this, deltaTime);

            if (transform.InRange(SET_startPosition, maxDistance)) return;
            
            var shouldBrake = _onOutOfRangeAction?.Invoke(this) ?? false;
            if (shouldBrake) return;
            
            _poolManager.ReleasePoolObject(this);
        }

        private void ManageAnimation()
        {
            if (!_animationSprites.CanCycle()) return;

            UPDATE_animationTimer += deltaTime;
            if (UPDATE_animationTimer < 1 / _animationSpeed) return;
            UPDATE_animationTimer = 0;

            projectileSpriteRenderer.sprite = _animationSprites.Next();
        }

        private void ManageLifeTime()
        {
            if (!_lifeTime.HasValue) return;
            
            UPDATE_currentLifeTime += deltaTime;
            if (UPDATE_currentLifeTime < _lifeTime.Value) return;
            _poolManager.ReleasePoolObject(this);
            _lifeTime = null;
        }

        public Projectile SetAdditionalData<T>(T data) where T : class, new()
        {
            _additionalData = data;
            return this;
        }

        public T GetAdditionalData<T>() where T : class, new()
        {
            _additionalData ??= new T();
            return (T)_additionalData;
        }

        private void OnTargetHit(CanBeDamaged hitObj)
        {
            var shouldBrake = _onHitAction?.Invoke(this, hitObj) ?? false;
            if (shouldBrake) return;

            var damageContext = PlayerManager.GetDamageContextManager().GetDamageContext(_damage, this, hitObj, _itemTags);
            hitObj.GetDamaged(damageContext.Damage);
            if (_pushForce.HasValue && hitObj is EnemyLogic enemyLogic) enemyLogic.PushEnemy(transform.position, _pushForce.Value);
            if (_effectType != EEffectType.None) hitObj.AddEffect(PlayerManager.GetEffectContextManager().GetEffectContext(_effectType, _effectTime, hitObj));
            
            if (_destroyOnCollision) _poolManager.ReleasePoolObject(this);
        }

        private void OnTargetHitStay(CanBeDamaged hitObj)
        {
            _onHitStayAction?.Invoke(this, hitObj);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, bulletRange);
        }

        public static bool CancelHit(Projectile _, CanBeDamaged __) => true;
    }
}