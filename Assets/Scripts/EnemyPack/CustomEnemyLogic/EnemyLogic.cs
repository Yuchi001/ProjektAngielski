using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using EnemyPack.Enums;
using EnemyPack.SO;
using ExpPackage;
using Managers;
using Managers.Enums;
using Other;
using Other.Interfaces;
using PlayerPack;
using UI;
using UnityEngine;
using WeaponPack.Other;

namespace EnemyPack.CustomEnemyLogic
{
    [RequireComponent(typeof(EnemyHealthBar))]
    public partial class EnemyLogic : CanBeDamaged
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
        
        [Header("Shoot")] 
        [SerializeField] private string playerTagName;
        [SerializeField] private LayerMask enemyProjectileLayerMask;
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int bulletDamage;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletsPerSec;
        
        private float _shootTimer = 0;

        public override int CurrentHealth => _currentHealth;

        private Collider2D Collider2D => GetComponent<Collider2D>();
        
        private SoEnemy _enemy;
        private Transform _target;
        private EnemySpawner _enemySpawner;

        private Vector3 _desiredDir;
        private Vector2 _desiredPos;

        private bool _toMerge = false;

        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        private static Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        
        private EnemyHealthBar EnemyHealthBar => GetComponent<EnemyHealthBar>();
       
        private bool _isBeingPushed = false;
        
        private int _currentHealth;
        public override int MaxHealth => Mathf.CeilToInt(_enemy.MaxHealth * _enemySpawner.EnemiesHpScale);

        private float _playerSpeed = 0;
        private float AttackRange => attackRange * _enemy.BodyScale;
        private float _collisionTimer = 0;
        private float Mass => Mathf.Pow(_enemy.BodyScale, 2);
        
        private float MovementSpeed => Slowed ? _enemy.MovementSpeed / 2f : _enemy.MovementSpeed;

        private EnemyLogic mergeParent;
        private int mergeCount = 1;

        private static PlayerEnchantments PlayerEnchantments => PlayerManager.Instance.PlayerEnchantments;
        
        public void Setup(SoEnemy enemy, Transform target, EnemySpawner enemySpawner)
        {
            _enemySpawner = enemySpawner;
            _enemy = Instantiate(enemy);
            _target = target;

            rb2d.mass = enemy.IsHeavy ? 999 : Mass;

            _desiredPos = transform.position;
            
            _currentHealth = MaxHealth;

            _playerSpeed = PlayerManager.Instance.PickedCharacter.MovementSpeed;

            Collider2D.isTrigger = (_enemy.EnemyState != EEnemyState.Chase && _enemy.EnemyState != EEnemyState.Stand) || _enemy.IsHeavy;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = aoc.animationClips.Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip)).ToList();
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            EnemyHealthBar.Setup(SpriteRenderer);
        }
        
        protected override void OnUpdate()
        {
            if (mergeParent) Merge();
            else switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    UpdateChaseBehaviour();
                    break;
                case EEnemyState.Follow:
                    UpdateFollowBehaviour();
                    break;
                case EEnemyState.Patrol:
                    UpdatePatrolBehaviour();
                    break;
                case EEnemyState.Stand:
                    break;
            }
            
            if (_enemy.CanShoot) UpdateShootBehaviour();

            ManagePlayerCollision();
            EnemyHealthBar.ManageHealthBar();
        }

        private float GetMovementSpeed()
        {
            return _enemy.EnemyState switch
            {
                EEnemyState.Chase => ChaseSpeed,
                EEnemyState.Stand => 0,
                EEnemyState.Follow => MovementSpeed,
                _ => MovementSpeed
            };
        }

        private void UpdateShootBehaviour()
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer < 1f / bulletsPerSec) return;

            _shootTimer = 0;
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(bulletDamage, bulletSpeed)
                .SetSprite(projectileSprites, 5, 4)
                .SetLightColor(Color.red)
                .SetTargetTag(playerTagName, 7)
                .SetDirection(PlayerManager.Instance.transform.position)
                .SetReady();
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

            if (mergeParent) transform.position = Vector2.MoveTowards(transform.position, mergeParent.transform.position, GetMovementSpeed() * 1.5f);
            else if (!_isBeingPushed) switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    FixedUpdateChaseBehaviour();
                    break;
                case EEnemyState.Follow:
                    FixedUpdateFollowBehaviour();
                    break;
                case EEnemyState.Patrol:
                    FixedUpdatePatrolBehaviour();
                    break;
                case EEnemyState.Stand:
                    break;
            }
        }

        private void Merge()
        {
            if (!mergeParent) {
                Collider2D.isTrigger = false;
                _toMerge = false;
                return;
            }

            if (Vector2.Distance(transform.position, mergeParent.transform.position) > 0.05f) return;
            
            mergeParent.MergeParent(mergeCount);
            Destroy(gameObject);
        }

        private void SetMerge(EnemyLogic enemyLogic)
        {
            _toMerge = true;
            mergeParent = enemyLogic;
            Collider2D.isTrigger = true;
        }

        private void MergeParent(int childMergeCount)
        {
            mergeCount++;
            transform.localScale += Vector3.one * (0.25f * childMergeCount);
            _currentHealth += _enemy.MaxHealth * childMergeCount;
        }

        private void ManagePlayerCollision()
        {
            var inDistance = Vector2.Distance(PlayerPos, transform.position) < AttackRange;
            _collisionTimer += Time.deltaTime;
            
            if (!inDistance) return;
            ManagePlayerDashCollision();

            if (_collisionTimer < 1f / attacksPerSecond) return;
            
            switch (_enemy.EnemyState)
            {
                case EEnemyState.Chase:
                    PlayerHitChaseBehaviour();
                    break;
                case EEnemyState.Follow:
                    break;
                case EEnemyState.Patrol:
                    break;
                case EEnemyState.Stand:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _collisionTimer = 0;
            PlayerHealth.GetDamaged(attackDamage);
        }

        private void ManagePlayerDashCollision()
        {
            if (!PlayerEnchantments.Has(EEnchantmentName.DashKill) || !PlayerManager.Instance.PlayerMovement.Dash) return;

            var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.DashKill, EValueKey.Percentage);
            if ((float)CurrentHealth / MaxHealth > percentage) return;
            GetDamaged(9999);
        }
        
        public void PushEnemy(Vector2 force, float time)
        {
            if (_isBeingPushed || mergeParent || _enemy.EnemyState == EEnemyState.Stand) return;
            
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

            var isCrit = Stuned && PlayerEnchantments.Has(EEnchantmentName.StunDoubleDamage); 
            if (isCrit) 
                value *= 2;
            
            AudioManager.Instance.PlaySound(ESoundType.EnemyHurt);
            
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicatorPrefab, value, isCrit);
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

            _enemySpawner.IncrementDeadEnemies(this);
            
            base.OnDie();
        }

        public void DieWithoutGem()
        {
            _target = null;
            rb2d.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            
            base.OnDie();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckEnemyCollision(other.gameObject);
            
            if (other.TryGetComponent(out Projectile projectile))
                projectile.ManageHit(gameObject);
            
            if (other.TryGetComponent(out IDamageEnemy damageEnemy)) 
                damageEnemy.TriggerDamage(this);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CheckEnemyCollision(other.gameObject);

            if (other.gameObject.TryGetComponent(out Projectile projectile))
                projectile.ManageHit(gameObject);
            
            if (other.gameObject.TryGetComponent(out IDamageEnemy damageEnemy)) 
                damageEnemy.TriggerDamage(this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out Projectile projectile)) 
                projectile.ManageCollisionStay(gameObject);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Projectile projectile)) 
                projectile.ManageCollisionStay(gameObject);
        }

        private void CheckEnemyCollision(GameObject hitObj)
        {
            if (_toMerge) return;
            
            if (!hitObj.TryGetComponent(out EnemyLogic enemyLogic)) return;

            if (enemyLogic._enemy.name != _enemy.name) return;

            enemyLogic.SetMerge(this);
        }
    }
}