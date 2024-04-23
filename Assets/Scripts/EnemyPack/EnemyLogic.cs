using System;
using System.Collections.Generic;
using EnemyPack.SO;
using ExpPackage;
using Other;
using PlayerPack;
using UI;
using UnityEngine;

namespace EnemyPack
{
    public class EnemyLogic : CanBeDamaged
    {
        [SerializeField] private float maxDistanceFromPlayer = 50f;
        [SerializeField] private GameObject damageIndicatorPrefab;
        [SerializeField] private GameObject expGemPrefab;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int attackDamage;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;

        private SoEnemy _enemy;
        private Transform _target;

        private Vector2 desiredDir;

        private float _timer = 0;
        private int _health;

        private Vector2 _startPosition;

        public void Setup(SoEnemy enemy, Transform target)
        {
            _startPosition = transform.position;
            _enemy = Instantiate(enemy);
            _target = target;
            _health = enemy.MaxHealth;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
        }

        private void Update()
        {
            if(Vector2.Distance(transform.position, _startPosition) >= maxDistanceFromPlayer)
                Destroy(gameObject);
            
            if (_target == null) return;
            
            _timer += Time.deltaTime;
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            Vector2 dir = _target.position - transform.position;
            dir.Normalize();
            desiredDir = dir;
        }

        private void FixedUpdate()
        {
            if (_target == null) return;
            
            rigidbody2D.velocity = desiredDir * _enemy.MovementSpeed;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth) 
                || _timer < 1 / attacksPerSecond) return;

            _timer = 0;
            playerHealth.GetDamaged(attackDamage);
        }

        public override void GetDamaged(int value)
        {
            base.GetDamaged(value);
            
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicatorPrefab, value);
            _health = Mathf.Clamp(_health - value, 0, _enemy.MaxHealth);
            if(_health <= 0) OnDie();
        }

        public override void OnDie()
        {
            ExpGem.SpawnExpGem(expGemPrefab, transform.position, _enemy.ExpGemType);
            
            base.OnDie();
        }
    }
}