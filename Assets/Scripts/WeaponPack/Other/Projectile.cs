using System;
using System.Collections.Generic;
using EnemyPack;
using EnemyPack.SO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace WeaponPack.Other
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer projectileSprite;
        [SerializeField] private Light2D light2D;
        [SerializeField] private new Collider2D collider2D;
        [SerializeField] private float maxDistance = 20f;
        private Transform _target;
        private int _damage;
        private float _speed;
        private GameObject _flightParticles;
        private GameObject _onHitParticles;

        private Action<GameObject> _onHit = null;

        private List<Sprite> _sprites = new();
        private float _animSpeed = 1f;
        private int _currentIndex = 0;
        private float? _rotationSpeed = null;
        private float? _range = null;

        private bool _ready = false;

        private float _timer = 0;

        private Vector2 _startDistance;

        private Action<GameObject> _deathBehaviour = Destroy;
        private Action<GameObject> _outOfRangeBehaviour = Destroy;

        #region Setup methods

        public Projectile Setup(int damage, float speed)
        {
            _range = maxDistance;
            var t = transform;
            _startDistance = t.position;
            UtilsMethods.LookAtMouse(t);
            _damage = damage;
            _speed = speed;
            return this;
        }

        public Projectile SetDontDestroyOnHit()
        {
            _deathBehaviour = null;
            return this;
        }

        public Projectile SetOutOfRangeBehaviour(Action<GameObject> outOfRangeBehaviour)
        {
            _outOfRangeBehaviour = outOfRangeBehaviour;
            return this;
        }

        public Projectile SetRange(float range)
        {
            _range = range;
            return this;
        }

        public Projectile SetRotationSpeed(float speed)
        {
            _rotationSpeed = speed;
            return this;
        }

        public Projectile SetCollisionHard()
        {
            collider2D.isTrigger = false;
            return this;
        }

        public Projectile SetSprite(Sprite sprite)
        {
            projectileSprite.sprite = sprite;
            return this;
        }
        
        public Projectile SetSprite(List<Sprite> sprites, float animSpeed)
        {
            _sprites.AddRange(sprites);
            _animSpeed = animSpeed;
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
            UtilsMethods.LookAtObj(target, _target.transform.position);
            return this;
        }

        public Projectile SetFlightParticles(GameObject flightParticles)
        {
            _flightParticles = Instantiate(flightParticles, transform.position, Quaternion.identity, transform);
            return this;
        }

        public Projectile SetOnHitParticles(GameObject onHitParticles)
        {
            _onHitParticles = onHitParticles;
            return this;
        }

        public Projectile SetDirection(Vector3 direction)
        {
            UtilsMethods.LookAtObj(transform, direction);
            return this;
        }

        public Projectile SetOnHitAction(Action<GameObject> onHit)
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
            
            AnimateProjectile();
            MoveProjectile();
            CheckDistance();
        }

        private void CheckDistance()
        {
            var distance = Vector2.Distance(transform.position, _startDistance);

            if (distance < _range) return;

            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                Destroy(particlesInstance, 2f);
            }
            _outOfRangeBehaviour.Invoke(gameObject);
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            ManageHit(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            ManageHit(other.gameObject);
        }

        private void ManageHit(GameObject hitObj)
        {
            if (!_ready) return;

            var isEnemyHit = hitObj.TryGetComponent<EnemyLogic>(out var enemyLogic);
            var isTargetHit = _target != null && hitObj.transform.GetInstanceID() == _target.GetInstanceID();
            if (!isEnemyHit && !isTargetHit) return;

            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                Destroy(particlesInstance, 2f);
            }
            
            _onHit?.Invoke(hitObj);

            if (_flightParticles != null)
            {
                _flightParticles.transform.SetParent(null);
                _flightParticles.transform.localScale = Vector2.one;
                _flightParticles.GetComponent<ParticleSystem>().Stop(true);
                Destroy(_flightParticles, 10f);
            }
            
            if(isEnemyHit) enemyLogic.GetDamaged(_damage);
            
            _deathBehaviour?.Invoke(gameObject);
        }
    }
}