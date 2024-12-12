using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using DamageIndicatorPack;
using EnchantmentPack.Enums;
using EnemyPack.Enums;
using EnemyPack.SO;
using ExpPackage;
using ItemPack.WeaponPack.Other;
using Managers.Enums;
using Other;
using Other.Interfaces;
using PlayerPack;
using PlayerPack.Enums;
using PoolPack;
using UnityEngine;

namespace EnemyPack.CustomEnemyLogic
{
    [RequireComponent(typeof(EnemyHealthBar))]
    public partial class EnemyLogic : CanBeDamaged
    {
        [Header("General")] 
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Animator animator;
        
        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        private static Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;
        private EnemyHealthBar EnemyHealthBar => GetComponent<EnemyHealthBar>();
        private Collider2D Collider2D => GetComponent<Collider2D>();
        public override int MaxHealth => Mathf.CeilToInt(_enemy.MaxHealth * _enemySpawner.EnemiesHpScale);
        private static float PlayerSpeed => PlayerManager.Instance.PlayerStatsManager.GetStat(EPlayerStatType.MovementSpeed);
        private float Mass => Mathf.Pow(_enemy.BodyScale, 2);
        private float MovementSpeed => Slowed ? _enemy.MovementSpeed / 2f : _enemy.MovementSpeed;
        private static PlayerEnchantments PlayerEnchantments => PlayerManager.Instance.PlayerEnchantments;
        

        private bool _isBeingPushed = false;
        
        public override int CurrentHealth => _currentHealth;
        private int _currentHealth;
        
        private SoEnemy _enemy;
        private EnemySpawner _enemySpawner;
        
        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _enemySpawner = poolManager as EnemySpawner;
        }

        public override void OnGet(SoPoolObject enemy)
        {
            base.OnGet(enemy);
            
            var player = PlayerManager.Instance;
            if (player == null) _enemySpawner.ReleasePoolObject(this);
            
            _isBeingPushed = false;
            
            _enemy = enemy.As<SoEnemy>();
            rb2d.mass = _enemy.IsHeavy ? 999 : Mass;
            
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
            if (Dead || Stuned || !Active) return;
            
            EnemyHealthBar.ManageHealthBar();
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
            if (_isBeingPushed || _enemy.EnemyState == EEnemyState.Stand) return;
            
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
            
            AudioManager.PlaySound(ESoundType.EnemyHurt);
            
            IndicatorManager.SpawnIndicator(transform.position, value, isCrit);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 0, MaxHealth);
            
            EnemyHealthBar.UpdateHealthBar(_currentHealth, MaxHealth);
            
            if(_currentHealth <= 0) OnDie();
        }

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            ExpPool.SpawnExpGem(_enemy.ExpGemType, transform.position);
            rb2d.velocity = Vector2.zero;
            Collider2D.enabled = false;
            
            base.OnDie(destroyObj, _enemySpawner);
        }

        public void DieWithoutGem()
        {
            rb2d.velocity = Vector2.zero;
            Collider2D.enabled = false;
            
            base.OnDie();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Projectile projectile))
                projectile.ManageHit(gameObject);
            
            if (other.TryGetComponent(out IDamageEnemy damageEnemy)) 
                damageEnemy.TriggerDamage(this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out Projectile projectile)) 
                projectile.ManageCollisionStay(gameObject);
        }

        private void OnDisable()
        {
            if (_enemySpawner == null) return;
            _enemySpawner.IncrementDeadEnemies(this, _enemy);
        }
    }
}