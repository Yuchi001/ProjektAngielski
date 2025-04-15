using System.Collections.Generic;
using System.Linq;
using AudioPack;
using DamageIndicatorPack;
using DifficultyPack;
using EnchantmentPack.Enums;
using EnemyPack.Enums;
using EnemyPack.SO;
using EnemyPack.States;
using InventoryPack.WorldItemPack;
using Managers.Enums;
using Other;
using PlayerPack;
using PoolPack;
using UnityEngine;

namespace EnemyPack
{
    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyLogic : CanBeDamaged
    {
        [Header("General")] 
        [SerializeField] private float knockbackDecay;
        [SerializeField] private Animator animator;

        public Animator Animator => animator;
        public static Vector2 PlayerPos => PlayerManager.PlayerPos;
        private EnemyHealthBar _enemyHealthBar;
        public override int MaxHealth => Mathf.CeilToInt(EnemyData.MaxHealth * DifficultyManager.EnemyHpScale);
        private static PlayerEnchantments PlayerEnchantments => PlayerManager.PlayerEnchantments;
        private Vector2 knockbackVelocity;

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
            _enemyHealthBar = GetComponent<EnemyHealthBar>();
            _enemySpawner = poolManager as EnemySpawner;
        }

        public override void OnGet(SoPoolObject enemy)
        {
            base.OnGet(enemy);
            
            SpriteRenderer.enabled = false;
            
            if (!PlayerManager.HasInstance()) _enemySpawner.ReleasePoolObject(this);
            
            _isBeingPushed = false;

            Animator.enabled = true;
            
            EnemyData = enemy.As<SoEnemy>();
            //rb2d.mass = Mass;
            
            _currentHealth = MaxHealth;
            

            var scale = EnemyData.BodyScale;
            transform.localScale = new Vector3(scale,scale,scale);
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = aoc.animationClips.Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, EnemyData.WalkingAnimationClip)).ToList();
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            _enemyHealthBar.Setup(SpriteRenderer);

            _currentState = StateFactory.GetState(EnemyData.EnemyBehaviour);
            _currentState.Reset(this);
            _currentState.Enter(this);
            
            SpriteRenderer.enabled = true;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            if (_enemySpawner == null) return;
            _enemySpawner.IncrementDeadEnemies(this, EnemyData);
        }

        public override void InvokeUpdate()
        {
            animator.speed = Stuned ? 0 : Slowed ? 0.5f : 1;
            if (Dead || (Stuned && _currentState.CanBeStunned) || !Active) return;

            if (_isBeingPushed)
            {
                _isBeingPushed = knockbackVelocity.magnitude > 0.01f;
                transform.position += (Vector3)(knockbackVelocity * deltaTime);
                knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackDecay * deltaTime);
                return;
            }

            var rotation = _currentState.GetRotation(this);
            if (rotation != ESpriteRotation.None)
            {
                var playerOnLeft = PlayerPos.x < transform.position.x;
                var rotationRight = rotation == ESpriteRotation.RotateRight;
                SpriteRenderer.flipX = playerOnLeft == rotationRight;
            }
            
            _enemyHealthBar.ManageHealthBar();
            
            _currentState.Execute(this);
        }

        public void SwitchState(StateBase state)
        {
            state.Enter(this);
            _currentState = state;
        }
        
        public void PushEnemy(Vector2 rootPos, float force)
        {
            if (_isBeingPushed || !_currentState.CanBePushed) return;

            knockbackVelocity = ((Vector2)transform.position - rootPos).normalized * force;
            _isBeingPushed = true;
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
            WorldItemManager.SpawnSouls(transform.position, EnemyData.Difficulty);

            base.OnDie(destroyObj, _enemySpawner);
        }

        public void DieWithoutGem()
        {
            base.OnDie();
        }
        
        //TODO: Dash kill mechanic on player
    }
}