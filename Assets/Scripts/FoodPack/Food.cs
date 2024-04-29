using System;
using Other.SO;
using PlayerPack;
using UnityEngine;

namespace FoodPack
{
    public class Food : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float range;
        
        private SoFood _food;

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        private PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        
        public void Setup(SoFood food)
        {
            _food = food;
            spriteRenderer.sprite = food.FoodSprite;
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, PlayerPos) > range) return;
            
            PlayerHealth.Heal(_food.SaturationValue);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}