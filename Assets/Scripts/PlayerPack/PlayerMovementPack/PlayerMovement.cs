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
        [SerializeField] private Animator animator;
        private SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;
        private PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();

        public bool LookingRight => _lookingRight;
        private bool _lookingRight;
        
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
            if (PlayerHealth.Dead) return;
            
            ManageMovement();
        }

        private void ManageMovement()
        {
            var velocity = GetVelocity();
            rb2d.velocity = velocity;
            animator.SetBool("isWalking", velocity != Vector2.zero);

            if (velocity.x == 0) return;

            _lookingRight = velocity.x > 0;
            Debug.Log(_lookingRight);
            playerSpriteTransform.rotation = Quaternion.Euler(0, _lookingRight ? 180 : 0, 0);
        }
    }
}