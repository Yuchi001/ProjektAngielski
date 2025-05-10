using System;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using DamageIndicatorPack;
using DifficultyPack;
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
        [SerializeField] private float knockbackTime;
        [SerializeField] private Animator animator;
        public Animator Animator => animator;
        public static Vector2 PlayerPos => PlayerManager.PlayerPos;
        private EnemyHealthBar _enemyHealthBar;
        public override int MaxHealth => Mathf.CeilToInt(EnemyData.MaxHealth * DifficultyManager.EnemyHpScale);
        private Vector2 knockbackVelocity;

        public bool Invincible { get; private set; }

        public void SetInvincible(bool invincible)
        {
            Invincible = invincible;
        }
        
        private float _pushTime = 0;
        
        public override int CurrentHealth => _currentHealth;
        private int _currentHealth;

        public SoEnemy EnemyData { get; private set; }

        private EnemySpawner _enemySpawner;

        private StateBase _currentState;

        private Vector3 _lastPos;
        
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

            _pushTime = 0;

            Animator.enabled = true;
            
            EnemyData = enemy.As<SoEnemy>();
            //rb2d.mass = Mass;

            Animator.speed = EnemyData.AnimationSpeed;
            _currentHealth = MaxHealth;

            var scale = EnemyData.BodyScale;
            transform.localScale = new Vector3(scale,scale,scale);
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = aoc.animationClips.Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, EnemyData.WalkingAnimationClip)).ToList();
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            _enemyHealthBar.Setup(SpriteRenderer);
            
            SpriteRenderer.enabled = true;
        }

        public void SetPosition(Vector2 spawnPos)
        {
            transform.position = spawnPos;
            _currentState = StateFactory.GetState(EnemyData.EnemyBehaviour, EnemyData);
            _currentState.Enter(this, _currentState);
            _lastPos = transform.position;
            EnemyManager.AddPos(this);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            if (_enemySpawner == null) return;
            EnemyManager.RegisterEnemyDeath(this);
            EnemyManager.RemovePos(this, _lastPos);
        }

        private void OnDestroy()
        {
            EnemyManager.RemovePos(this, _lastPos);
        }

        private void Update()
        {
            if (Dead || !Active) return;
            
            if (_lastPos != transform.position) EnemyManager.UpdatePos(this, _lastPos);
            _lastPos = transform.position;
            
            if (_pushTime > 0)
            {
                _pushTime -= Time.deltaTime;
                transform.position += (Vector3)(knockbackVelocity * Time.deltaTime);
                return;
            }

            var rotation = _currentState.GetRotation(this);
            if (rotation != ESpriteRotation.None)
            {
                var playerOnLeft = PlayerPos.x < transform.position.x;
                var rotationRight = rotation == ESpriteRotation.RotateRight;
                SpriteRenderer.flipX = playerOnLeft == rotationRight;
            }
            
            _currentState.Execute(this);
        }

        protected override void LazyUpdate(float lazyDeltaTime)
        {
            animator.speed = Stuned ? 0 : Slowed ? EnemyData.AnimationSpeed / 2f : EnemyData.AnimationSpeed;
            if (Dead || (Stuned && _currentState.CanBeStunned) || !Active) return;
            
            _enemyHealthBar.ManageHealthBar();
            
            _currentState.LazyExecute(this, lazyDeltaTime);
        }

        public bool SwitchState(StateBase state)
        {
            if (state == _currentState) return false;
            
            _currentState.Exit(this);
            state.Enter(this, _currentState);
            _currentState = state;
            return true;
        }
        
        public void PushEnemy(Vector2 rootPos, float force)
        {
            if (_pushTime > 0 || !_currentState.CanBePushed) return;

            knockbackVelocity = ((Vector2)transform.position - rootPos).normalized * force;
            _pushTime = knockbackTime;
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            if (Invincible || Dead) return;
            
            base.GetDamaged(value, color);
            
            AudioManager.PlaySound(ESoundType.EnemyHurt);
            
            IndicatorManager.SpawnIndicator(transform.position, value, false); //TODO: maybe add crit chance
            _currentHealth = Mathf.Clamp(_currentHealth - value, 0, MaxHealth);
            
            _enemyHealthBar.UpdateHealthBar(_currentHealth, MaxHealth);
            
            if (_currentHealth <= 0) OnDie();
        }

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            if (Dead) return;
            
            WorldItemManager.SpawnSouls(transform.position, EnemyData.GetSoulDropCount());
            var scrapCount = EnemyData.GetScrapDropCount();
            if (scrapCount > 0) WorldItemManager.SpawnScraps(transform.position, scrapCount);

            base.OnDie(destroyObj, _enemySpawner);
        }

        public void DieWithoutGem()
        {
            if (Dead) return;
            
            base.OnDie();
        }
    }
}