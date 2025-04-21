using System;
using System.Collections.Generic;
using EnemyPack;
using ItemPack.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Serialization;
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
        public float DeltaTime => deltaTime;

        // RESET VALUES
        private float RESET_bulletRange;
        private string RESET_sortingLayer;
        private int RESET_sortingOrder;
        private float RESET_maxDistance;

        // SET VALUES
        private Vector2 SET_startPosition;
        
        // UPDATE VALUES
        private float UPDATE_currentLifeTime;

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

        public Projectile SetSprite(Sprite sprite)
        {
            projectileSpriteRenderer.sprite = sprite;
            projectileSpriteRenderer.gameObject.SetActive(true);
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
            _targetDetector = new TargetDetector(transform, transform.localScale.x * bulletRange, _targetTag);
            _targetDetector.SetOnTriggerEnter(OnTargetHit);
            _targetDetector.SetOnTriggerStay(OnTargetHitStay);
            SET_startPosition = transform.position;
            UPDATE_currentLifeTime = 0;
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
            _damage = 0;
            _itemTags = new List<EItemTag>();
            _pushForce = null;
            _lifeTime = null;
            _destroyOnCollision = true;
            transform.localScale = Vector3.one;
            trailRenderer.gameObject.SetActive(false);
            projectileSpriteRenderer.gameObject.SetActive(false);
            maxDistance = RESET_maxDistance;
            bulletRange = RESET_bulletRange;
            projectileSpriteRenderer.sortingLayerName = RESET_sortingLayer;
            projectileSpriteRenderer.sortingOrder = RESET_sortingOrder;
            _effectType = EEffectType.None;
            _effectTime = 0;
            _targetTag = "";
            base.OnGet(so);
        }

        public override void OnRelease()
        {
            _targetDetector = null;
            _onHitAction = null;
            _onHitStayAction = null;
            _onUpdateAction = null;
            _additionalData = null;
            _onOutOfRangeAction = null;
            base.OnRelease();
        }

        public override void InvokeUpdate()
        {
            base.InvokeUpdate();
            if (_lifeTime.HasValue)
            {
                UPDATE_currentLifeTime += deltaTime;
                if (UPDATE_currentLifeTime >= _lifeTime.Value)
                {
                    _poolManager.ReleasePoolObject(this);
                    _lifeTime = null;
                }
            }
            
            _onUpdateAction?.Invoke(this);
            _projectileMovementStrategy.MoveProjectile(this);

            if (transform.InRange(SET_startPosition, maxDistance)) return;
            
            var shouldBrake = _onOutOfRangeAction?.Invoke(this) ?? false;
            if (shouldBrake) return;
            
            _poolManager.ReleasePoolObject(this);
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

            var damageContext = PlayerManager.GetDamageContextManager().GetDamageContext(_damage, hitObj, _itemTags);
            hitObj.GetDamaged(damageContext.Damage);
            if (_pushForce.HasValue && hitObj is EnemyLogic enemyLogic) enemyLogic.PushEnemy(transform.position, _pushForce.Value);
            if (_effectType != EEffectType.None) hitObj.AddEffect(PlayerManager.GetEffectContextManager().GetEffectContext(_effectType, _effectTime, hitObj));
            
            if (_destroyOnCollision) _poolManager.ReleasePoolObject(this);
        }

        private void OnTargetHitStay(CanBeDamaged hitObj)
        {
            _onHitStayAction?.Invoke(this, hitObj);
        }
    }
}