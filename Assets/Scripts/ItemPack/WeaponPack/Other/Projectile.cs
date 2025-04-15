using System;
using System.Collections.Generic;
using AccessorPack;
using EnemyPack;
using Managers;
using Other;
using Other.Enums;
using PlayerPack;
using PoolPack;
using ProjectilePack;
using UnityEngine;
using Utils;
using TransformExtensions = Utils.TransformExtensions;

namespace ItemPack.WeaponPack.Other
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private SpriteRenderer projectileSprite;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float bulletRange;
        
        private readonly List<Sprite> _sprites = new();

        private Transform _target;
        private GameObject _flightParticles;
        private GameObject _onHitParticles;
        
        private float _onHitParticlesScale = 1;
        private int _damage;
        private int _currentIndex;
        
        private float _speed;
        private float _animSpeed = 1f;
        private float _timer;

        private float? _rotationSpeed;
        private float? _range;

        private float? _lifeTime;
        private float? _pushForce;

        private Vector2? _startLerpPosition;
        private Vector2? _lerpDestination;
        private float? _lerpTime;

        private float _currentLerpTime;
        
        private float _currentLifeTime;

        private string _targetTag = "Enemy";
        
        private bool _ready;

        private Vector2 _startDistance;

        private EEffectType _effectType = EEffectType.None;
        private float _effectDuration;

        private Action<CanBeDamaged, Projectile> _onCollisionStay;
        private Action<CanBeDamaged, Projectile> _onHit;
        private Action<Projectile> _deathBehaviour = projectile => Destroy(projectile.gameObject);
        private Action<Projectile> _outOfRangeBehaviour = projectile => Destroy(projectile.gameObject);
        private Action<Projectile> _update;

        private bool _damageOnHit = true;

        private readonly Dictionary<string, float> _customValues = new();

        private TriggerDetector _triggerDetector;

        #region Setup methods

        public Projectile Setup(int damage, float speed)
        {
            trailRenderer.gameObject.SetActive(false);
            _range = maxDistance;
            var t = transform;
            _startDistance = t.position;
            _damage = damage;
            _speed = speed;
            _triggerDetector = new TriggerDetector(transform, bulletRange)
                .SetOnTriggerEnter(OnTargetHit)
                .SetOnTriggerStay(OnTargetStay);
            return this;
        }

        public Projectile SetTrail(float time = 0.1f)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.time = time;
            return this;
        }

        public Projectile SetLifeTime(float lifeTime)
        {
            _lifeTime = lifeTime;
            return this;
        }

        public Projectile SetTargetTag(string targetTag, int layerIndex = 3)
        {
            gameObject.layer = layerIndex;
            _targetTag = targetTag;
            return this;
        }

        public Projectile SetNewCustomValue(string id, float value = 0)
        {
            _customValues.Add(id, value);
            return this;
        }

        public Projectile SetDontDestroyOnHit()
        {
            _deathBehaviour = null;
            return this;
        }

        public Projectile SetSpeed(float speed)
        {
            _speed = speed;
            return this;
        }

        public Projectile SetDisableDamageOnHit()
        {
            _damageOnHit = false;
            return this;
        }

        public Projectile SetUpdate(Action<Projectile> update)
        {
            _update = update;
            return this;
        }

        public Projectile SetOutOfRangeBehaviour(Action<Projectile> outOfRangeBehaviour)
        {
            _outOfRangeBehaviour = outOfRangeBehaviour;
            return this;
        }

        public Projectile SetRange(float range)
        {
            _range = range;
            return this;
        }

        public Projectile SetEffect(EEffectType effectType, float time)
        {
            _effectType = effectType;
            _effectDuration = time;
            return this;
        }

        public Projectile SetRotationSpeed(float speed)
        {
            _rotationSpeed = speed;
            return this;
        }

        public Projectile SetSprite(Sprite sprite, float spriteScale = 1)
        {
            projectileSprite.sprite = sprite;
            projectileSprite.transform.localScale = new Vector3(spriteScale, spriteScale, 0);
            if (sprite == null) projectileSprite.enabled = false;
            return this;
        }

        public Projectile SetSortingLayer(string sortingLayerName, int index = 0)
        {
            projectileSprite.sortingLayerName = sortingLayerName;
            projectileSprite.sortingOrder = index;
            return this;
        }
        
        public Projectile SetSprite(IEnumerable<Sprite> sprites, float animSpeed, float spriteScale = 1)
        {
            _sprites.AddRange(sprites);
            _animSpeed = animSpeed;
            projectileSprite.transform.localScale = new Vector3(spriteScale, spriteScale, 0);
            return this;
        }

        public Projectile SetTarget(Transform target)
        {
            _target = target;
            TransformExtensions.LookAt(transform, _target.transform.position);
            return this;
        }

        public Projectile SetOnCollisionStay(Action<CanBeDamaged, Projectile> onCollisionStay)
        {
            _onCollisionStay = onCollisionStay;
            return this;
        }

        public Projectile SetFlightParticles(GameObject flightParticles, float scale = 1, bool setOnTop = false)
        {
            var cachedTransform = transform;
            _flightParticles = Instantiate(flightParticles, cachedTransform.position, Quaternion.identity, cachedTransform);
            _flightParticles.transform.localScale = new Vector3(scale, scale, 0);
            _flightParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = setOnTop ? 1 : 0;
            return this;
        }

        public Projectile SetOnHitParticles(GameObject onHitParticles, float scale = 1)
        {
            _onHitParticles = onHitParticles;
            _onHitParticlesScale = scale;
            return this;
        }

        public Projectile SetDirection(Vector3 direction, float rotateBy = 0, bool checkX = false)
        {
            TransformExtensions.LookAt(transform, direction);
            transform.Rotate(0, 0, rotateBy);
            if(checkX && direction.x < transform.position.x) 
                projectileSprite.transform.Rotate(0, 0, 180);
            return this;
        }

        public Projectile SetSpriteRotation(float angle)
        {
            projectileSprite.transform.Rotate(0, 0, angle);
            return this;
        }
        
        public Projectile SetStaticSpriteRotation(float angle)
        {
            var projectileSpriteTransform = projectileSprite.transform;
            var newRot = projectileSpriteTransform.rotation;
            newRot.z = angle;
            projectileSpriteTransform.rotation = newRot;
            return this;
        }
        
        public Projectile SetPushForce(float force)
        {
            _pushForce = force;
            return this;
        }

        public Projectile SetOnHitAction(Action<CanBeDamaged, Projectile> onHit)
        {
            _onHit = onHit;
            return this;
        }

        public Projectile SetLerp(Vector2 position, float time)
        {
            _startLerpPosition = transform.position;
            _lerpDestination = position;
            _lerpTime = time;
            _currentLerpTime = 0;
            return this;
        }
        
        public Projectile SetLerp(Transform lerpTo, float time)
        {
            _startLerpPosition = transform.position;
            _target = lerpTo;
            _lerpTime = time;
            _currentLerpTime = 0;
            return this;
        }

        public Projectile SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, 0);
            return this;
        }

        public void SetReady()
        {
            _ready = true;
            projectileSprite.gameObject.SetActive(true);
        }

        #endregion

        private void Update()
        {
            if (!_ready) return;
 
            if (_rotationSpeed != null)
            {
                projectileSprite.transform.Rotate(0, 0, _rotationSpeed.Value);
            }

            _update?.Invoke(this);
            
            AnimateProjectile();
            MoveProjectile();
            CheckDistance();
            CheckLifeTime();
            _triggerDetector.CheckForTriggers();
        }
        
        public float GetCustomValue(string id)
        {
            if (!_customValues.ContainsKey(id)) return -1;
            return _customValues[id];
        }
        
        public void SetCustomValue(string id, float newValue)
        {
            if (!_customValues.ContainsKey(id)) return;
            _customValues[id] = newValue;
        }

        private void CheckLifeTime()
        {
            if (_lifeTime == null) return;
            
            _currentLifeTime += Time.deltaTime;
            if (_lifeTime > _currentLifeTime) return;
            
            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                particlesInstance.transform.localScale =
                    new Vector3(_onHitParticlesScale, _onHitParticlesScale, _onHitParticlesScale);
                Destroy(particlesInstance, 2f);
            }
            _outOfRangeBehaviour.Invoke(this);
        }

        private void CheckDistance()
        {
            if (transform.InRange(_startDistance, _range ?? 0)) return;

            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                particlesInstance.transform.localScale =
                    new Vector3(_onHitParticlesScale, _onHitParticlesScale, _onHitParticlesScale);
                Destroy(particlesInstance, 2f);
            }
            _outOfRangeBehaviour.Invoke(this);
        }

        private void MoveProjectile()
        {
            var projectileTransform = transform;
            Vector2 newPos;
            if ((_startLerpPosition.HasValue || _target != null) && _lerpTime.HasValue)
            {
                _currentLerpTime += Time.deltaTime;
                var time = Mathf.Clamp01(_currentLerpTime / _lerpTime.Value);
                var dest = _lerpDestination ?? _target.position;
                newPos = Vector2.Lerp(_startLerpPosition.Value, dest, time);
            }
            else if (_target != null) newPos = Vector2.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            else newPos = projectileTransform.position + projectileTransform.up * (_speed * Time.deltaTime);

            transform.position = newPos;
        }

        private void AnimateProjectile()
        {
            _timer += Time.deltaTime;
            if (_timer < 1 / _animSpeed || _sprites.Count <= 1) return;

            _timer = 0;
            _currentIndex++;
            if (_currentIndex >= _sprites.Count) _currentIndex = 0;

            projectileSprite.sprite = _sprites[_currentIndex];
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return projectileSprite;
        }

        public void OnTargetStay(CanBeDamaged hitObj)
        {
            if (!hitObj.CompareTag(_targetTag)) return;
            
            if (!_ready) return;
            
            _onCollisionStay?.Invoke(hitObj, this);
        }

        public void OnTargetHit(CanBeDamaged enemy)
        {
            if (!enemy.CompareTag(_targetTag)) return;
            
            if (!_ready) return;
            
            _onHit?.Invoke(enemy, this);
            
            if (_effectType != EEffectType.None) enemy.AddEffect(_effectType, _effectDuration);
            
            TryPush(enemy);

            ManageParticlesOnHit();
            
            if (_damageOnHit) enemy.GetDamaged(_damage);
            
            _deathBehaviour?.Invoke(this);
        }

        private void TryPush(CanBeDamaged hitEntity)
        {
            if (_pushForce == null) return;
            
            var enemy = hitEntity as EnemyLogic;
            if (enemy == null) return;
            
            enemy.PushEnemy(transform.position, _pushForce.Value);
        }

        private void ManageParticlesOnHit()
        {
            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                particlesInstance.transform.localScale =
                    new Vector3(_onHitParticlesScale, _onHitParticlesScale, _onHitParticlesScale);
                Destroy(particlesInstance, 2f);
            }

            if (_flightParticles == null) return;
            
            _flightParticles.transform.SetParent(null);
            _flightParticles.transform.localScale = Vector2.one;
            _flightParticles.GetComponent<ParticleSystem>().Stop(true);
            Destroy(_flightParticles, 10f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, bulletRange);
        }
    }
}