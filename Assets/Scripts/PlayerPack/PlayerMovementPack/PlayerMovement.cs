using System.Collections.Generic;
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
        private SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;
        private PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();

        public bool LookingRight => _lookingRight;
        private bool _lookingRight;

        private bool _dash = false;
        private float _dashTimer = 0;
        
        private void Awake()
        {
            animator.speed = animationSpeed;
            _buttonsActive = new Dictionary<KeyCode, bool>
            {
                { UpBind, false },
                { LeftBind, false },
                { DownBind, false },
                { RightBind, false },
            };
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
            _dashTimer += Time.deltaTime;
            if (_dash)
            {
                if (_dashTimer < dashTime) return;
                
                _dash = false;
                PlayerHealth.Invincible = false;
                rb2d.velocity /= dashForceMultiplier;
                _dashTimer = 0;
                return;
            }

            if (!Input.GetKey(KeyCode.Space) || _dashTimer < dashCooldown) return;
            
            _dashTimer = 0;
            _dash = true;
            rb2d.velocity *= dashForceMultiplier;
            PlayerHealth.Invincible = true;
        }
    }
}