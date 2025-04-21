using System;
using Other;
using Other.Enums;
using PoolPack;
using UnityEngine;

namespace ProjectilePack
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : PoolObject
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private SpriteRenderer projectileSprite;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float bulletRange;

        private float RESET_bulletRange;
        private float RESET_maxDistance;

        private IProjectileMovementStrategy _projectileMovementStrategy;
        private TriggerDetector _triggerDetector;

        private int _damage;
        private string _targetTag;
        
        // ADDITIONAL ACTIONS
        private Action<Projectile, CanBeDamaged> _onHitAction;
        private Action<Projectile, CanBeDamaged> _onHitStayAction;
        private Action<Projectile> _onUpdateAction;

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

        public Projectile SetOnHitAction(Action<Projectile, CanBeDamaged> onHitAction)
        {
            _onHitAction = onHitAction;
            return this;
        }
        
        public Projectile SetOnHitStayAction(Action<Projectile, CanBeDamaged> onHitStayAction)
        {
            _onHitStayAction = onHitStayAction;
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
            _triggerDetector = new TriggerDetector(transform, transform.localScale.x * bulletRange, _targetTag);
            _triggerDetector.SetOnTriggerEnter(OnTargetHit);
            _triggerDetector.SetOnTriggerStay(OnTargetHitStay);
            return this;
        }

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            RESET_bulletRange = bulletRange;
            RESET_maxDistance = maxDistance;
        }

        public override void OnGet(SoPoolObject so)
        {
            _damage = 0;
            trailRenderer.gameObject.SetActive(false);
            projectileSprite.gameObject.SetActive(false);
            maxDistance = RESET_maxDistance;
            bulletRange = RESET_bulletRange;
            _effectType = EEffectType.None;
            _effectTime = 0;
            _targetTag = "";
            base.OnGet(so);
        }

        public override void OnRelease()
        {
            _triggerDetector = null;
            _onHitAction = null;
            _onHitStayAction = null;
            _onUpdateAction = null;
            _additionalData = null;
            base.OnRelease();
        }

        private void Update()
        {
            _onUpdateAction?.Invoke(this);
            _projectileMovementStrategy.MoveProjectile(transform);
        }

        public void SetAdditionalData<T>(T data) where T : class, new()
        {
            _additionalData = data;
        }

        public T GetAdditionalData<T>() where T : class, new()
        {
            return (T)_additionalData;
        }

        private void OnTargetHit(CanBeDamaged hitObj)
        {
            _onHitAction?.Invoke(this, hitObj);
        }

        private void OnTargetHitStay(CanBeDamaged hitObj)
        {
            _onHitStayAction?.Invoke(this, hitObj);
        }
    }
}