using System;
using System.Collections;
using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using UnityEngine;
using Utils;

namespace WeaponPack.Other
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private LineCollision lineCollision;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject laserParticlesPrefab;
        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;

        private Vector2 _target;

        private float _animTime = 0.3f;
        private float _maxScale = 1;
        private float _duration = 0;
        private float _damageRate = 2;

        private int _damage = 0;

        private bool _enableCollisions = false;

        private readonly List<ParticleSystem> _spawnedParticles = new();

        #region Setup methods

        public Laser Setup(int damage, float damageRate)
        {
            _damage = damage;
            _damageRate = damageRate;
            
            lineRenderer.SetPosition(0, PlayerPos);
            
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;

            lineCollision.PolygonCollider2D.enabled = false;
            
            return this;
        }

        public Laser SetAnimTime(float animTime)
        {
            _animTime = animTime;
            return this;
        }

        public Laser SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }

        public Laser SetTargetPosition(Vector2 target)
        {
            _target = target;
            lineRenderer.SetPosition(1, _target);
            return this;
        }

        public Laser SetMaxScale(float scale)
        {
            _maxScale = scale;
            return this;
        }

        public void Ready()
        {
            LeanTween.value(gameObject, 0, _maxScale, _animTime)
                .setEaseInCubic()
                .setOnUpdate((value) =>
                {
                    lineRenderer.startWidth = value;
                    lineRenderer.endWidth = value;
                }).setOnComplete(() =>
                {
                    StartCoroutine(HandleLaserDamage(lineRenderer));
                });
            
            var startParticles = SpawnParticles(PlayerPos, _target);
            var endParticles = SpawnParticles(_target, PlayerPos);
            
            _spawnedParticles.Add(startParticles);
            _spawnedParticles.Add(endParticles);
        }

        private ParticleSystem SpawnParticles(Vector2 spawnPos, Vector2 lookAtPos)
        {
            var particles = Instantiate(laserParticlesPrefab, spawnPos, Quaternion.identity, transform).GetComponent<ParticleSystem>();
            var angle = UtilsMethods.GetAngleToObject(particles.transform, lookAtPos);
            var shape = particles.shape;
            shape.radius = _maxScale / 2;
            particles.transform.rotation = Quaternion.Euler(0, 0, angle -90);

            return particles;
        }

        #endregion
        
        
        private IEnumerator HandleLaserDamage(LineRenderer lineRenderer)
        {
            lineCollision.UpdateCollisions();
            lineCollision.PolygonCollider2D.enabled = true;
            
            float _timer = 0;
            float _rateTimer = 999;
            while (_timer < _duration)
            {
                yield return new WaitForEndOfFrame();
                
                _rateTimer += Time.deltaTime;
                _timer += Time.deltaTime;
                if (_rateTimer < 1f / _damageRate) continue;

                _enableCollisions = true;
                _rateTimer = 0;
            }
            LeanTween.value(gameObject, _maxScale, 0, _animTime)
                .setEaseOutCubic()
                .setOnUpdate((value) =>
                {
                    lineRenderer.startWidth = value;
                    lineRenderer.endWidth = value;
                    
                    _spawnedParticles.ForEach(p =>
                    {
                        var shape = p.shape;
                        shape.radius = value / 2;
                    });
                }).setOnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ManageCollision(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            ManageCollision(other.gameObject);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!_enableCollisions) return;
            
            ManageCollision(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!_enableCollisions) return;
            
            ManageCollision(other.gameObject);
        }

        private void ManageCollision(GameObject hitObj)
        {
            var success = hitObj.TryGetComponent<EnemyLogic>(out var enemyLogic);
            if (!success) return;
            
            enemyLogic.GetDamaged(_damage);

            _enableCollisions = false;
        }
    }
}