using System;
using Managers.Base;
using Other;
using Other.SO;
using PlayerPack;
using PoolPack;
using UnityEngine;

namespace FoodPack
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Food : PoolObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float range;
        
        private SoFood _food;
        private FoodSpawner _foodSpawner;

        private CircleCollider2D CircleCollider2D => GetComponent<CircleCollider2D>();
        private PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            CircleCollider2D.radius = range;
            //CircleCollider2D.isTrigger = true;
            _foodSpawner = (FoodSpawner)poolManager;
            _food = (SoFood)_foodSpawner.GetRandomPoolData();
            spriteRenderer.sprite = _food.FoodSprite;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            PlayerHealth.Heal(_food.SaturationValue);
            _foodSpawner.ReleasePoolObject(this);
        }
    }
}