using System;
using System.Collections.Generic;
using System.Globalization;
using EnemyPack;
using EnemyPack.SO;
using Managers;
using Managers.Other;
using Other;
using Other.Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace WeaponPack.Other
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private SpriteRenderer projectileSprite;
        [SerializeField] private Light2D light2D;
        [SerializeField] private new Collider2D collider2D;
        [SerializeField] private float maxDistance = 20f;
        
        private readonly List<Sprite> _sprites = new();

        private Transform _target;
        private GameObject _flightParticles;
        private GameObject _onHitParticles;

        private bool _destroyOnContactWithWall = true;
        public bool DestroyOnContactWithWall => _destroyOnContactWithWall;

        private float _onHitParticlesScale = 1;
        private int _damage;
        private int _currentIndex = 0;
        
        private float _speed;
        private float _animSpeed = 1f;
        private float _timer = 0;

        private float? _rotationSpeed = null;
        private float? _range = null;

        private float? _lifeTime = null;
        private float? _pushForce = null;
        
        private float _currentLifeTime = 0;

        private string _targetTag = "Enemy";
        
        private bool _ready = false;

        private Vector2 _startDistance;

        private EffectInfo _effectInfo = null;

        private Action<GameObject, Projectile> _onCollisionStay = null;
        private Action<GameObject, Projectile> _onHit = null;
        private Action<Projectile> _deathBehaviour = projectile => Destroy(projectile.gameObject);
        private Action<Projectile> _outOfRangeBehaviour = projectile => Destroy(projectile.gameObject);
        private Action<Projectile> _update = null;

        private bool _damageOnHit = true;

        private readonly Dictionary<string, float> _customValues = new();

        #region Setup methods

        public Projectile Setup(int damage, float speed)
        {
            trailRenderer.gameObject.SetActive(false);
            _range = maxDistance;
            var t = transform;
            _startDistance = t.position;
            _damage = damage;
            _speed = speed;
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

        public Projectile SetDisableDestroyOnContactWithWall()
        {
            _destroyOnContactWithWall = false;
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
            _effectInfo = new EffectInfo
            {
                effectType = effectType,
                time = time,
            };
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

        public Projectile SetLightColor(Color color)
        {
            light2D.color = color;
            return this;
        }

        public Projectile SetTarget(Transform target)
        {
            _target = target;
            UtilsMethods.LookAtObj(transform, _target.transform.position);
            return this;
        }

        public Projectile SetOnCollisionStay(Action<GameObject, Projectile> onCollisionStay)
        {
            _onCollisionStay = onCollisionStay;
            return this;
        }

        public Projectile SetFlightParticles(GameObject flightParticles, float scale = 1, bool setOnTop = false)
        {
            _flightParticles = Instantiate(flightParticles, transform.position, Quaternion.identity, transform);
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
            UtilsMethods.LookAtObj(transform, direction);
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
        
        public Projectile SetPushForce(float force)
        {
            _pushForce = force;
            return this;
        }

        public Projectile SetOnHitAction(Action<GameObject, Projectile> onHit)
        {
            _onHit = onHit;
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
            var distance = Vector2.Distance(transform.position, _startDistance);

            if (distance < _range) return;

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
            var newPos = _target == null ? 
                (Vector2)(projectileTransform.position + projectileTransform.up * (_speed * Time.deltaTime)) :
                Vector2.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);

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

        public void ManageCollisionStay(GameObject hitObj)
        {
            if (!hitObj.CompareTag(_targetTag)) return;
            
            if (!_ready) return;

            var isEnemyHit = hitObj.TryGetComponent<CanBeDamaged>(out var enemyLogic);
            if (!isEnemyHit) return;
            
            _onCollisionStay?.Invoke(hitObj, this);
        }

        public void ManageHit(GameObject hitObj)
        {
            if (!hitObj.CompareTag(_targetTag)) return;
            
            if (!_ready) return;

            var isHit = hitObj.TryGetComponent<CanBeDamaged>(out var hitEntity);
            if (!isHit) return;
            
            _onHit?.Invoke(hitObj, this);
            
            if (_effectInfo != null) hitEntity.AddEffect(_effectInfo);
            
            TryPush(hitEntity, hitObj);

            ManageParticlesOnHit();
            
            if (_damageOnHit) hitEntity.GetDamaged(_damage);
            
            _deathBehaviour?.Invoke(this);
        }

        private void TryPush(CanBeDamaged hitEntity, GameObject hitObj)
        {
            if (_pushForce == null) return;
            
            var enemy = hitEntity as EnemyLogic;
            
            if (enemy == null) return;
            
            var diff = hitObj.transform.position - transform.position;
            diff = diff.normalized;
            enemy.PushEnemy(diff * _pushForce.Value, 0.3f);
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
    }
}