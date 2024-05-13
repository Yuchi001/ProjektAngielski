using System;
using System.Collections.Generic;
using EnemyPack.SO;
using ExpPackage;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack;
using UI;
using UnityEngine;
using WeaponPack.Other;

namespace EnemyPack
{
    public class EnemyLogic : CanBeDamaged
    {
        [Header("General")]
        
        [SerializeField] private float maxDistanceFromPlayer = 10f;
        [SerializeField] private GameObject damageIndicatorPrefab;
        [SerializeField] private GameObject expGemPrefab;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int attackDamage;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;
        
        [Header("Shooting")]
        
        [SerializeField] private GameObject projectilePrefab;

        private SoEnemy _enemy;
        private Transform _target;

        private Vector2 _desiredDir;

        private float _collisionTimer = 0;
        private float _actionTimer = 0;
        private int _health;

        private EnemySpawner _enemySpawner;

        private int MaxHealth => Mathf.CeilToInt(_enemy.MaxHealth * _enemySpawner.EnemiesHpScale);

        private bool _outOfRange = false;

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;

        public void Setup(SoEnemy enemy, Transform target, EnemySpawner enemySpawner)
        {
            rigidbody2D.mass = enemy.BodyScale * 10;

            _outOfRange = false;
            _enemySpawner = enemySpawner;
            _enemy = Instantiate(enemy);
            _target = target;
            
            _health = MaxHealth;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
        }

        public void Despawn(Vector3 newPos)
        {
            _outOfRange = false;
            transform.position = newPos;
        }
        protected override void OnUpdate()
        {
            if (_outOfRange) _enemySpawner.AddOutOfRangeEnemy(this);
            
            _outOfRange = Vector2.Distance(transform.position, PlayerPos) >= maxDistanceFromPlayer;
            
            if (_target == null) return;

            _collisionTimer += Time.deltaTime;
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            Vector2 dir = _target.position - transform.position;
            dir.Normalize();
            _desiredDir = dir;
        }

        private void HandleAction()
        {
            if (!_enemy.UseAction) return;

            _actionTimer += Time.deltaTime;
            if (_actionTimer < _enemy.ActionCooldown) return;

            _actionTimer = 0;
            
        }

        private void FixedUpdate()
        {
            if (_target == null) return;
            
            rigidbody2D.velocity = _desiredDir * _enemy.MovementSpeed;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth) 
                || _collisionTimer < 1 / attacksPerSecond) return;

            _collisionTimer = 0;
            playerHealth.GetDamaged(attackDamage);
        }

        public override void GetDamaged(int value)
        {
            base.GetDamaged(value);
            
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