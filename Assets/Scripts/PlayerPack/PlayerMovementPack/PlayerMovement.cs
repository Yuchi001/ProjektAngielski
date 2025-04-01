using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using PlayerPack.Enums;
using PlayerPack.SO;
using UnityEngine;

namespace PlayerPack.PlayerMovementPack
{
    [RequireComponent(typeof(PlayerHealth))]
    public partial class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Transform playerSpriteTransform;
        [SerializeField] private float animationSpeed = 0.5f;
        [SerializeField] private float dashForceMultiplier = 4;
        [SerializeField] private float dashTime = 0.5f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private Animator animator;
        
        private PlayerStatsManager PlayerStatsManager => PlayerManager.Instance.PlayerStatsManager;
        private SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;
        private PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();
        private float MovementSpeed => PlayerStatsManager.GetStat(EPlayerStatType.MovementSpeed);
        public int MaxDashStacks => PlayerStatsManager.GetStatAsInt(EPlayerStatType.DashStacks) + _additionalDashStacks;
        private int _additionalDashStacks = 0;

        private float _additionalMovementSpeed = 0;

        public bool LookingRight { get; private set; }

        public bool DuringDash { get; private set; } = false;
        private float _dashTimer = 0;
        private float _dashingTimer = 0;

        private PlayerDashAnim _playerDashAnim;
        
        public int CurrentDashStacks { get; private set; } = 0;

        public delegate void PlayerDashEndDelegate();
        public static event PlayerDashEndDelegate OnPlayerDashEnd;

        public delegate void PlayerDashIncrement(int value);
        public static event PlayerDashIncrement OnPlayerDashIncrement;

        private void Start()
        {
            animator.speed = animationSpeed;
            _dashTimer = dashCooldown;
            CurrentDashStacks = MaxDashStacks;
            _buttonsActive = new Dictionary<KeyCode, bool>
            {
                { GameManager.UpBind, false },
                { GameManager.LeftBind, false },
                { GameManager.DownBind, false },
                { GameManager.RightBind, false },
            };
            _playerDashAnim = GetComponent<PlayerDashAnim>();
        }
        public float GetDashProgress()
        {
            return _dashTimer / dashCooldown;
        }

        public void AddDashStack(int value = 1)
        {
            _additionalDashStacks += value;
            OnPlayerDashIncrement?.Invoke(value);
        }

        /// <summary>
        /// Modifies current maxSpeed by given value;
        /// </summary>
        /// <param name="percentage">Percentage in format ranging from 0.0 -? 0% to 1.0 -> 100%</param>
        public void ModifySpeedByPercentage(float percentage)
        {
            _additionalMovementSpeed += MovementSpeed * percentage;
        }

        protected void Update()
        {
            if(Time.timeScale == 0) return;
            
            if (PlayerHealth.Dead) rb2d.velocity = Vector2.zero;
            else ManageMovement();
        }

        private void ManageMovement()
        {
            ManageDash();
            if (DuringDash) return;
            
            var velocity = GetVelocity();
            rb2d.velocity = velocity;
            animator.SetBool("isWalking", velocity != Vector2.zero);

            if (velocity.x == 0) return;

            LookingRight = velocity.x > 0;
            playerSpriteTransform.rotation = Quaternion.Euler(0, LookingRight ? 180 : 0, 0);
        }

        private void ManageDash()
        {
            if (PlayerManager.Instance.CurrentState == PlayerManager.State.IN_TAVERN) return;
            
            if (DuringDash)
            {
                _dashingTimer += Time.deltaTime;
                if (_dashingTimer < dashTime) return;
                
                _playerDashAnim.StopAnim();
                DuringDash = false;
                PlayerHealth.Invincible = false;
                rb2d.velocity /= dashForceMultiplier;
                _dashingTimer = 0;
                ResetKeys();
                OnPlayerDashEnd?.Invoke();
            }

            if (CurrentDashStacks <= MaxDashStacks) _dashTimer += Time.deltaTime;
            
            if (_dashTimer >= dashCooldown)
            {
                CurrentDashStacks += CurrentDashStacks < MaxDashStacks ? 1 : 0;
                _dashTimer = 0;
            }

            if (!Input.GetKeyDown(GameManager.DeclineBind) || CurrentDashStacks == 0 || rb2d.velocity == Vector2.zero) return;

            if (CurrentDashStacks == MaxDashStacks) _dashTimer = 0;
            
            _playerDashAnim.StartAnim();
            DuringDash = true;
            CurrentDashStacks--;
            rb2d.velocity = rb2d.velocity.normalized * dashForceMultiplier;
            PlayerHealth.Invincible = true;
        }
    }
}