using System.Collections.Generic;
using PlayerPack.SO;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace PlayerPack.PlayerMovementPack
{
    public partial class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Transform playerSpriteTransform;
        [SerializeField] private float animationSpeed = 0.5f;
        [SerializeField] private Animator animator;
        private SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;
        
        
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

        private void Update()
        {
            ManageMovement();
        }

        private void ManageMovement()
        {
            playerSpriteTransform.rotation = Quaternion.Euler(0, transform.position.x > UtilsMethods.GetMousePosition().x ? 0 : 180, 0);

            var movement = GetMovement();
            
            animator.SetBool("isWalking", movement != Vector2.zero);
            
            var position = transform.position;
            position += (Vector3)movement;

            transform.position = position;
        }
    }
}