using Other.SO;
using PlayerPack;
using UnityEngine;

namespace FoodPack
{
    public class Food : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private SoFood _food;
        
        public void Setup(SoFood food)
        {
            _food = food;
            spriteRenderer.sprite = food.FoodSprite;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerHealth>(out var playerHealth)) return;
            
            playerHealth.Heal(_food.SaturationValue);
        }
    }
}