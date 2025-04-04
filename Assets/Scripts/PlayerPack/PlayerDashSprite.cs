using UnityEngine;

namespace PlayerPack
{
    public class PlayerDashSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void Setup(float lifeTime)
        {
            var lookingRight = PlayerManager.PlayerMovement.LookingRight;
            spriteRenderer.sprite = PlayerManager.PickedCharacter.CharacterSprite;
            var rot = transform.rotation;
            rot.y = lookingRight ? 180 : 0;
            transform.rotation = rot;
            Destroy(gameObject, lifeTime);
        }
    }
}