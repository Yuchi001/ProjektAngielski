using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using DamageIndicatorPack;
using EnchantmentPack.Enums;
using EnemyPack.Enums;
using EnemyPack.SO;
using EnemyPack.States;
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

        public Animator Animator => animator;
        public static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        public static Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;
        private EnemyHealthBar _enemyHealthBar;
        private Collider2D _collider2D;
        public override int MaxHealth => Mathf.CeilToInt(EnemyData.MaxHealth * _enemySpawner.EnemiesHpScale);
        private static float PlayerSpeed => PlayerManager.Instance.PlayerStatsManager.GetStat(EPlayerStatType.MovementSpeed);
        private float Mass => Mathf.Pow(EnemyData.BodyScale, 2);
        private float MovementSpeed => Slowed ? EnemyData.MovementSpeed / 2f : EnemyData.MovementSpeed;
        private static PlayerEnchantments PlayerEnchantments => PlayerManager.Instance.PlayerEnchantments;

        public bool Invincible { get; private set; }

        public void SetInvincible(bool invincible)
        {
            Invincible = invincible;
        }

        private bool _isBeingPushed = false;
        
        public override int CurrentHealth => _currentHealth;
        private int _currentHealth;

        public SoEnemy EnemyData { get; private set; }

        private EnemySpawner _enemySpawner;

        private StateBase _currentState;
        
        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _collider2D = GetComponent<Collider2D>();
            _enemyHealthBar = GetComponent<EnemyHealthBar>();
            _enemySpawner = poolManager as EnemySpawner;
        }

        public override void OnGet(SoPoolObject enemy)
        {
            base.OnGet(enemy);
            
            var player = PlayerManager.Instance;
            if (player == null) _enemySpawner.ReleasePoolObject(this);
            
            _isBeingPushed = false;
            
            EnemyData = enemy.As<SoEnemy>();
            rb2d.mass = Mass;
            
            _currentHealth = MaxHealth;
            
            _collider2D.enabled = true;

            var scale = EnemyData.BodyScale;
            transform.localScale = new Vector3(scale,scale,scale);
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = aoc.animationClips.Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, EnemyData.WalkingAnimationClip)).ToList();
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            CanBeDamagedSetup();
            
            _enemyHealthBar.Setup(SpriteRenderer);

            _currentState = StateFactory.GetState(EnemyData.EnemyBehaviour);
            _currentState.Reset(this);
            _currentState.Enter(this);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            if (_enemySpawner == null) return;
            _enemySpawner.IncrementDeadEnemies(this, EnemyData);
        }

        public override void InvokeUpdate()
        {
            if (Dead || (Stuned && _currentState.CanBeStunned) || !Active) return;

            if (EnemyData.SpriteRotation != ESpriteRotation.None)
            {
                var playerOnLeft = PlayerPos.x < transform.position.x;
                SpriteRenderer.flipX = playerOnLeft == (EnemyData.SpriteRotation == ESpriteRotation.RotateRight);
            }
            
            _enemyHealthBar.ManageHealthBar();
            
            _currentState.Execute(this);
        }

        private void FixedUpdate()
        {
            if (!Active) return;
            
            animator.speed = Stuned ? 0 : Slowed ? 0.5f : 1;
            rb2d.mass = Stuned ? 999 : Slowed ? Mass * 2 : Mass;

            if (Stuned) _currentState.FixedExecute(this);
            else rb2d.velocity = Vector2.zero;
        }

        public void SwitchState(StateBase state)
        {
            state.Enter(this);
            _currentState = state;
        }
        
        public void PushEnemy(Vector2 force, float time)
        {
            if (_isBeingPushed || !_currentState.CanBePushed) return;
            
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
            if (isCrit) value *= 2;
            
            AudioManager.PlaySound(ESoundType.EnemyHurt);
            
            IndicatorManager.SpawnIndicator(transform.position, value, isCrit);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 0, MaxHealth);
            
            _enemyHealthBar.UpdateHealthBar(_currentHealth, MaxHealth);
            
            if(_currentHealth <= 0) OnDie();
        }

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            ExpPool.SpawnExpGem(EnemyData.ExpGemType, transform.position);
            rb2d.velocity = Vector2.zero;
            _collider2D.enabled = false;
            
            base.OnDie(destroyObj, _enemySpawner);
        }

        public void DieWithoutGem()
        {
            rb2d.velocity = Vector2.zero;
            _collider2D.enabled = false;
            
            base.OnDie();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") 
                && PlayerManager.Instance.PlayerMovement.DuringDash
                && PlayerEnchantments.Has(EEnchantmentName.DashKill))
            {
                var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.DashKill, EValueKey.Percentage);
                if ((float)CurrentHealth / MaxHealth > percentage) return;
                GetDamaged(9999);
                return;
            }
            
            if (other.TryGetComponent(out Projectile projectile))
                projectile.ManageHit(gameObject);
            
            else if (other.TryGetComponent(out IDamageEnemy damageEnemy)) 
                damageEnemy.TriggerDamage(this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out Projectile projectile)) 
                projectile.ManageCollisionStay(gameObject);
        }
    }
}