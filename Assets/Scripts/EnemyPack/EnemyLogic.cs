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
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EnemyPack
{
    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyLogic : CanBeDamaged
    {
        [Header("General")] 
        [SerializeField] private float attackRange = 0.3f;
        [SerializeField] private GameObject damageIndicatorPrefab;
        [SerializeField] private GameObject expGemPrefab;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int attackDamage;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Animator animator;
        public override int CurrentHealth => _currentHealth;
        
        private SoEnemy _enemy;
        private Transform _target;
        private EnemySpawner _enemySpawner;

        private Vector3 _desiredDir;
        private Vector2 _desiredPos;

        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        private static Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        
        private EnemyHealthBar EnemyHealthBar => GetComponent<EnemyHealthBar>();
       
        private bool _isBeingPushed = false;
        
        private int _currentHealth;
        private int MaxHealth => Mathf.CeilToInt(_enemy.MaxHealth * _enemySpawner.EnemiesHpScale);
        
        private float AttackRange => attackRange * _enemy.BodyScale;
        private float _collisionTimer = 0;
        private float Mass => Mathf.Pow(_enemy.BodyScale, 2);
        private float MovementSpeed => Slowed ? _enemy.MovementSpeed / 2f : _enemy.MovementSpeed;
        
        
        public void Setup(SoEnemy enemy, Transform target, EnemySpawner enemySpawner)
        {
            _enemySpawner = enemySpawner;
            _enemy = Instantiate(enemy);
            _target = target;

            rb2d.mass = enemy.IsHeavy ? 999 : Mass;

            _desiredPos = transform.position;
            
            _currentHealth = MaxHealth;

            GetComponent<Collider2D>().isTrigger = _enemy.EnemyState != EEnemyState.Chase;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            _enemy.SpawnEnemyLogic(this);
            
            EnemyHealthBar.Setup(SpriteRenderer);
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

            ManagePlayerCollision();
            EnemyHealthBar.ManageHealthBar();
        }

        private void FixedUpdate()
        {
            animator.speed = Stuned ? 0 : Slowed ? 0.5f : 1;
            rb2d.mass = Stuned ? 999 : Slowed ? Mass * 2 : Mass;
            if (Stuned)
            {
                rb2d.velocity = Vector2.zero;
                return;
            }
            
            if (_isBeingPushed) return;
            
            switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    if (_target == null) return;
                    rb2d.velocity = _desiredDir * MovementSpeed;
                    break;
                case EEnemyState.Patrol:
                    rb2d.velocity = _desiredDir * MovementSpeed;
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
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            var dir = _target.position - transform.position;
            dir.Normalize();
            _desiredDir = dir;
        }

        private void ManagePlayerCollision()
        {
            _collisionTimer += Time.deltaTime;
            
            if (_collisionTimer < 1f / attacksPerSecond) return;

            if (Vector2.Distance(PlayerPos, transform.position) >= AttackRange) return;
            
            _collisionTimer = 0;
            PlayerHealth.GetDamaged(attackDamage);
        }
        
        public void PushEnemy(Vector2 force, float time)
        {
            if (_isBeingPushed) return;
            
            rb2d.AddForce(force, ForceMode2D.Impulse);
            _isBeingPushed = true;
            StartCoroutine(PushCoroutine(time));
        }

        private IEnumerator PushCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _isBeingPushed = false;
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            base.GetDamaged(value, color);
            
            
            AudioManager.Instance.PlaySound(ESoundType.EnemyHurt);
            
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicatorPrefab, value);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 0, MaxHealth);
            
            EnemyHealthBar.UpdateHealthBar(_currentHealth, MaxHealth);
            
            if(_currentHealth <= 0) OnDie();
        }

        public override void OnDie(bool destroyObj = true)
        {
            ExpGem.SpawnExpGem(expGemPrefab, transform.position, _enemy.ExpGemType);
            _target = null;
            rb2d.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            _enemySpawner.DeadEnemies++;
            
            base.OnDie();
        }

        public void DieWithoutGem()
        {
            _target = null;
            rb2d.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            
            base.OnDie();
        }
    }
}