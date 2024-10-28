using Managers.Base;
using Other;
using Other.SO;
using PlayerPack;
using UnityEngine;

namespace FoodPack
{
    public class Food : EntityBase
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float range;
        
        private SoFood _food;

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        private PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;

        private void Update()
        {
            if (Vector2.Distance(transform.position, PlayerPos) > range) return;
            
            PlayerHealth.Heal(_food.SaturationValue);
            
            DestroyNonAloc();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        public override void Setup(SoEntityBase soData)
        {
            _food = soData.As<SoFood>();
            spriteRenderer.sprite = _food.FoodSprite;
            SpawnNonAloc();
        }

        public override void SpawnSetup(SpawnerBase spawner)
        {
            // not needed
        }
    }
}