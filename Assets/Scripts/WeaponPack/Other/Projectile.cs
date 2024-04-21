using System;
using System.Collections.Generic;
using EnemyPack;
using UnityEngine;
using Utils;

namespace WeaponPack.Other
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : MonoBehaviour
    {
        private EnemyLogic _target;
        private Transform Target => _target.transform;
        private int _damage;
        private float _speed;
        private GameObject _flightParticles;
        private GameObject _onHitParticles;
        private SpriteRenderer _spriteRenderer;

        private List<Sprite> _sprites = new();
        private float _animSpeed = 1f;
        private int _currentIndex = 0;

        private bool _ready = false;

        private float _timer = 0;
        
        public Projectile Setup(int damage, float speed)
        {
            UtilsMethods.LookAtMouse(transform);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _damage = damage;
            _speed = speed;
            return this;
        }

        public Projectile SetSprite(Sprite sprite)
        {
            _sprites.Add(sprite);
            return this;
        }
        
        public Projectile SetSprite(List<Sprite> sprites, float animSpeed)
        {
            _sprites.AddRange(sprites);
            _animSpeed = animSpeed;
            return this;
        }

        public Projectile SetTarget(EnemyLogic enemyLogic)
        {
            _target = enemyLogic;
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

        public Projectile SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, 0);
            return this;
        }

        public void SetReady()
        {
            _ready = true;
        }

        private void Update()
        {
            if (!_ready) return;

            var newPos = _target == null ? 
                (Vector2)(transform.position + transform.forward * _speed) :
                Vector2.MoveTowards(transform.position, Target.position, _speed * Time.deltaTime);

            transform.position = newPos;
            
            _timer += Time.deltaTime;
            if (_timer < 1 / _animSpeed) return;

            _timer = 0;
            _currentIndex++;
            if (_currentIndex >= _sprites.Count) _currentIndex = 0;

            _spriteRenderer.sprite = _sprites[_currentIndex];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_ready) return;

            if (!other.gameObject.TryGetComponent<EnemyLogic>(out var enemyLogic)) return;

            if (_onHitParticles != null)
            {
                var particlesInstance = Instantiate(_onHitParticles, transform.position, Quaternion.identity);
                Destroy(particlesInstance, 2f);
            }

            if (_flightParticles != null)
            {
                _flightParticles.transform.parent = null;
                _flightParticles.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                Destroy(_flightParticles, 2f);
            }
            
            enemyLogic.GetDamaged(_damage);
            Destroy(gameObject);
        }
    }
}