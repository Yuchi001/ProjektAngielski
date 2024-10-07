using System;
using System.Collections.Generic;
using Managers;
using PlayerPack.SO;
using UnityEngine;
using UnityEngine.Serialization;

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
        private SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;
        private PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();
        private int MaxDashStacks => PickedCharacter.MaxDashStacks;

        private bool _lookingRight;

        private bool _dash = false;
        private float _dashTimer = 0;
        private float _dashingTimer = 0;
        public int CurrentDashStacks { get; private set; } = 0;
        
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
        }
        public float GetDashProgress()
        {
            return _dashTimer / dashCooldown;
        }

        protected void Update()
        {
            if (PlayerHealth.Dead) rb2d.velocity = Vector2.zero;
            else ManageMovement();
        }

        private void ManageMovement()
        {
            ManageDash();
            if (_dash) return;
            
            var velocity = GetVelocity();
            rb2d.velocity = velocity;
            animator.SetBool("isWalking", velocity != Vector2.zero);

            if (velocity.x == 0) return;

            _lookingRight = velocity.x > 0;
            playerSpriteTransform.rotation = Quaternion.Euler(0, _lookingRight ? 180 : 0, 0);
        }

        private void ManageDash()
        {
            if (_dash)
            {
                var sr = new GameObject("dashGhost", typeof(SpriteRenderer));
                sr.GetComponent<SpriteRenderer>().sprite = PickedCharacter.CharacterSprite;
                sr.transform.rotation = transform.GetChild(1).rotation;
                sr.transform.position = transform.position;
                Destroy(sr.gameObject, 0.1f);
                
                _dashingTimer += Time.deltaTime;
                if (_dashingTimer < dashTime) return;
                
                _dash = false;
                PlayerHealth.Invincible = false;
                rb2d.velocity /= dashForceMultiplier;
                _dashingTimer = 0;
                return;
            }

            if (CurrentDashStacks <= MaxDashStacks) _dashTimer += Time.deltaTime;
            
            if (_dashTimer >= dashCooldown)
            {
                CurrentDashStacks += CurrentDashStacks < MaxDashStacks ? 1 : 0;
                _dashTimer = 0;
            }

            if (!Input.GetKeyDown(GameManager.DeclineBind) || CurrentDashStacks == 0) return;

            if (CurrentDashStacks == MaxDashStacks) _dashTimer = 0;
            _dash = true;
            CurrentDashStacks--;
            rb2d.velocity = rb2d.velocity.normalized * dashForceMultiplier;
            PlayerHealth.Invincible = true;
        }
    }
}