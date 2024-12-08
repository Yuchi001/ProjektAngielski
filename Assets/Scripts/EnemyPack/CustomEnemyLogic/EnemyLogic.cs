using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using EnemyPack.Enums;
using EnemyPack.SO;
using ExpPackage;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using Other;
using Other.Interfaces;
using PlayerPack;
using PlayerPack.Enums;
using PoolPack;
using UI;
using UnityEngine;

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
        [SerializeField] private List<Sprite> projectileSprites;
        [SerializeField] private int bulletDamage;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletsPerSec;
        
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

        private static float PlayerSpeed => PlayerManager.Instance.PlayerStatsManager.GetStat(EPlayerStatType.MovementSpeed);

        private float AttackRange => attackRange * _enemy.BodyScale;
        private float Mass => Mathf.Pow(_enemy.BodyScale, 2);
        
        private float MovementSpeed => Slowed ? _enemy.MovementSpeed / 2f : _enemy.MovementSpeed;

        private EnemyLogic _mergeParent;
        private int mergeCount = 1;

        private const string COLLISION_TIMER_ID = "COLLISION_TIMER_ID";
        private const string SHOOT_TIMER_ID = "SHOOT_TIMER_ID";

        private static PlayerEnchantments PlayerEnchantments => PlayerManager.Instance.PlayerEnchantments;

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _enemySpawner = poolManager as EnemySpawner;
        }

        public override void OnGet(SoEntityBase enemy)
        {
            base.OnGet(enemy);
            
            var player = PlayerManager.Instance;
            if (player == null) _enemySpawner.ReleasePoolObject(this);

            _mergeParent = null;
            _toMerge = false;

            SetTimer(COLLISION_TIMER_ID);
            SetTimer(SHOOT_TIMER_ID);
            _isBeingPushed = false;

            mergeCount = 0;

            _enemy = enemy.As<SoEnemy>();
            _target = player.transform;

            rb2d.mass = _enemy.IsHeavy ? 999 : Mass;

            _desiredPos = transform.position;
            
            _currentHealth = MaxHealth;
            
            Collider2D.enabled = true;

            var scale = _enemy.BodyScale;
            transform.localScale = new Vector3(scale,scale,scale);

            Collider2D.isTrigger = (_enemy.EnemyState != EEnemyState.Chase && _enemy.EnemyState != EEnemyState.Stand) || _enemy.IsHeavy;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = aoc.animationClips.Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, _enemy.WalkingAnimationClip)).ToList();
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            CanBeDamagedSetup();
            
            EnemyHealthBar.Setup(SpriteRenderer);
        }
        
        public override void InvokeUpdate()
        {
            if (_effectsManager == null) return;
            
            if (Dead || Stuned || !Active) return;
            
            if (_mergeParent) Merge();
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
            
            if (_enemy.ShootType != EShootType.None) UpdateShootBehaviour();

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
            var count = _enemySpawner.ShootingEnemiesCount;
            if (count <= 0) count = 1;
            count = _enemy.ScaleShooting ? count : 1;
            if (CheckTimer(SHOOT_TIMER_ID) < count / bulletsPerSec) return;

            SetTimer(SHOOT_TIMER_ID);
            switch (_enemy.ShootType)
            {
                case EShootType.OneBullet: ShootOneBullet();
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!Active) return;
            
            animator.speed = Stuned ? 0 : Slowed ? 0.5f : 1;
            rb2d.mass = Stuned ? 999 : Slowed ? Mass * 2 : Mass;
            
            if (Stuned)
            {
                rb2d.velocity = Vector2.zero;
                return;
            }

            if (_mergeParent) transform.position = Vector2.MoveTowards(transform.position, _mergeParent.transform.position, GetMovementSpeed() * 1.5f);
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
            if (!_mergeParent) {
                Collider2D.isTrigger = false;
                _toMerge = false;
                return;
            }

            if (Vector2.Distance(transform.position, _mergeParent.transform.position) > 0.05f) return;
            
            _mergeParent.MergeParent(mergeCount);
            _enemySpawner.ReleasePoolObject(this);
        }

        private void SetMerge(EnemyLogic enemyLogic)
        {
            _toMerge = true;
            _mergeParent = enemyLogic;
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
            
            if (!inDistance) return;
            ManagePlayerDashCollision();

            if (CheckTimer(COLLISION_TIMER_ID) < 1f / attacksPerSecond) return;
            
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
            
            SetTimer(COLLISION_TIMER_ID);
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
            if (_isBeingPushed || _mergeParent || _enemy.EnemyState == EEnemyState.Stand) return;
            
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

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            var gem = (ExpGem)ExpPool.Instance.GetPoolObject();
            gem.Setup(_enemy.ExpGemType, transform.position);
            _target = null;
            rb2d.velocity = Vector2.zero;
            Collider2D.enabled = false;
            
            base.OnDie(destroyObj, _enemySpawner);
        }

        public void DieWithoutGem()
        {
            _target = null;
            rb2d.velocity = Vector2.zero;
            Collider2D.enabled = false;
            
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

        private void OnDisable()
        {
            if (_enemySpawner == null) return;
            _enemySpawner.IncrementDeadEnemies(this, _enemy);
        }
    }
}