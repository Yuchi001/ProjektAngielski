using System;
using System.Collections;
using Managers;
using PlayerPack.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerPack
{
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Transform playerSpriteTransform;
        [SerializeField] private float animationSpeed = 0.5f;
        [SerializeField] private float dashForceMultiplier = 4;
        [SerializeField] private float dashTime = 0.5f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private Animator animator;
        
        private PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();
        private static float MovementSpeed => PlayerManager.PlayerStatsManager.GetStat(EPlayerStatType.MovementSpeed);
        public int MaxDashStacks => PlayerManager.PlayerStatsManager.GetStatAsInt(EPlayerStatType.DashStacks) + _additionalDashStacks;
        private int _additionalDashStacks = 0;

        private float _additionalMovementSpeed = 0;

        public bool LookingRight { get; private set; }

        public bool DuringDash { get; private set; } = false;
        private float _dashTimer = 0;
        private float _dashingTimer = 0;

        private Vector2 _movementInput;
        private PlayerDashAnim _playerDashAnim;
        
        public int CurrentDashStacks { get; private set; } = 0;

        public delegate void PlayerDashEndDelegate();
        public static event PlayerDashEndDelegate OnPlayerDashEnd;

        public delegate void PlayerDashIncrement(int value);
        public static event PlayerDashIncrement OnPlayerDashIncrement;

        private void Awake()
        {
            PlayerManager.OnPlayerReady += Init;
            PlayerManager.OnChangeState += OnChangeState;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerReady -= Init;
            PlayerManager.OnChangeState -= OnChangeState;
        }

        private void Init()
        {
            animator.speed = animationSpeed;
            _dashTimer = dashCooldown;
            CurrentDashStacks = MaxDashStacks;
            _playerDashAnim = GetComponent<PlayerDashAnim>();
        }

        private static PlayerMovement PlayerMovementComp => PlayerManager.PlayerMovement;
        public static bool CanDash() => PlayerManager.CurrentState == PlayerManager.State.ON_MISSION && 
                                        PlayerMovementComp.CurrentDashStacks > 0 && 
                                        PlayerMovementComp.rb2d.velocity != Vector2.zero;

        private void OnChangeState(PlayerManager.State state)
        {
            CurrentDashStacks = MaxDashStacks;
            _dashTimer = dashCooldown;
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
        
        public void OnMove(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        private Vector2 GetVelocity()
        {
            if (_movementInput == Vector2.zero) return Vector2.zero;
        
            var movement = _movementInput.normalized;
            if (Mathf.Approximately(Mathf.Abs(movement.x) + Mathf.Abs(movement.y), 2))
            {
                movement /= Mathf.Sqrt(2);
            }
        
            return movement * (MovementSpeed + _additionalMovementSpeed);
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
            if (!CanDash()) return;
            
            if (DuringDash)
            {
                _dashingTimer += Time.deltaTime;
                if (_dashingTimer < dashTime) return;
                
                _playerDashAnim.StopAnim();
                DuringDash = false;
                PlayerHealth.Invincible = false;
                rb2d.velocity /= dashForceMultiplier;
                _dashingTimer = 0;
                OnPlayerDashEnd?.Invoke();
            }

            if (CurrentDashStacks <= MaxDashStacks) _dashTimer += Time.deltaTime;

            if (_dashTimer < dashCooldown) return;
            
            CurrentDashStacks += CurrentDashStacks < MaxDashStacks ? 1 : 0;
            _dashTimer = 0;
        }

        public void OnDashButtonClicked(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started || !CanDash()) return;

            if (CurrentDashStacks == MaxDashStacks) _dashTimer = 0;
            
            _playerDashAnim.StartAnim();
            DuringDash = true;
            CurrentDashStacks--;
            rb2d.velocity = rb2d.velocity.normalized * dashForceMultiplier;
            PlayerHealth.Invincible = true;
        }
    }
}