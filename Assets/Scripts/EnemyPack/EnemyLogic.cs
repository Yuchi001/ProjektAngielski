using System;
using System.Collections;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.SO;
using ExpPackage;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack;
using UI;
using UnityEngine;
using WeaponPack.Other;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    public class EnemyLogic : CanBeDamaged
    {
        [Header("General")]
        
        [SerializeField] private GameObject damageIndicatorPrefab;
        [SerializeField] private GameObject expGemPrefab;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int attackDamage;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;
        
        private SoEnemy _enemy;
        private Transform _target;

        private Vector3 _desiredDir;
        private Vector2 _desiredPos;

        private float _collisionTimer = 0;
        private int _health;

        private float MovementSpeed => Slowed ? _enemy.MovementSpeed / 2f : _enemy.MovementSpeed;

        private EnemySpawner _enemySpawner;
        private int MaxHealth => Mathf.CeilToInt(_enemy.MaxHealth * _enemySpawner.EnemiesHpScale);
        
        private bool _isBeingPushed = false;
        private float Mass => Mathf.Pow(_enemy.BodyScale, 2);

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;

        public void Setup(SoEnemy enemy, Transform target, EnemySpawner enemySpawner)
        {
            _enemySpawner = enemySpawner;
            _enemy = Instantiate(enemy);
            _target = target;

            rigidbody2D.mass = enemy.IsHeavy ? 999 : Mass;

            _desiredPos = transform.position;
            
            _health = MaxHealth;

            GetComponent<Collider2D>().isTrigger = _enemy.EnemyState != EEnemyState.Chase;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            _enemy.SpawnEnemyLogic(this);
        }

        public void PushEnemy(Vector2 force, float time)
        {
            if (_isBeingPushed) return;
            
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            _isBeingPushed = true;
            StartCoroutine(PushCoroutine(time));
        }

        private IEnumerator PushCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _isBeingPushed = false;
        }
        
        protected override void OnUpdate()
        {
            switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    Chase();
                    break;
                case EEnemyState.Patrol:
                    Patrol();
                    break;
                case EEnemyState.DontMove:
                    break;
            }
        }
        
        private void FixedUpdate()
        {
            animator.speed = Stuned ? 0 : Slowed ? 0.5f : 1;
            rigidbody2D.mass = Stuned ? 999 : Slowed ? Mass * 2 : Mass;
            if (Stuned)
            {
                rigidbody2D.velocity = Vector2.zero;
                return;
            }
            
            if (_isBeingPushed) return;
            
            switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    if (_target == null) return;
                    rigidbody2D.velocity = _desiredDir * MovementSpeed;
                    break;
                case EEnemyState.Patrol:
                    rigidbody2D.velocity = _desiredDir * MovementSpeed;
                    break;
                case EEnemyState.DontMove:
                    break;
            }
        }

        private void Patrol()
        {
            var currentPos = (Vector2)transform.position;
            if (Vector2.Distance(currentPos, _desiredPos) <= 0.1f)
            {
                var boundsX = GameManager.Instance.BoundaryX;
                var boundsY = GameManager.Instance.BoundaryY;

                var randomX = Random.Range(-boundsX, boundsX + 0.1f);
                var randomY = Random.Range(-boundsY, boundsY + 0.1f);

                _desiredPos = new Vector2(randomX, randomY);
            }
            
            var dir = _desiredPos - currentPos;
            dir.Normalize();
            _desiredDir = dir;
        }

        private void Chase()
        {
            if (_target == null) return;

            _collisionTimer += Time.deltaTime;
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            var dir = _target.position - transform.position;
            dir.Normalize();
            _desiredDir = dir;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth) 
                || _collisionTimer < 1 / attacksPerSecond) return;

            _collisionTimer = 0;
            playerHealth.GetDamaged(attackDamage);
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            base.GetDamaged(value, color);
            
            AudioManager.Instance.PlaySound(ESoundType.EnemyHurt);
            
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicatorPrefab, value);
            _health = Mathf.Clamp(_health - value, 0, MaxHealth);
            if(_health <= 0) OnDie();
        }

        public override void OnDie(bool destroyObj = true)
        {
            ExpGem.SpawnExpGem(expGemPrefab, transform.position, _enemy.ExpGemType);
            _target = null;
            rigidbody2D.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            _enemySpawner.DeadEnemies++;
            
            base.OnDie();
        }

        public void DieWithoutGem()
        {
            _target = null;
            rigidbody2D.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            
            base.OnDie();
        }
    }
}