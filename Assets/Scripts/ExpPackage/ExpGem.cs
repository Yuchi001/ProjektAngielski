using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using ExpPackage.Enums;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack;
using PoolPack;
using UnityEngine;

namespace ExpPackage
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExpGem :  PoolObject
    {
        [SerializeField] private float range;
        [SerializeField] private float animTime;
        [SerializeField] private List<ExpGemInfo> expAmountPair = new();
        [SerializeField] private List<ExpGemInfo> betterGemAmountPair = new();
        private static PlayerEnchantments PlayerEnchantments =>
            PlayerManager.Instance.PlayerEnchantments;

        private static Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        
        private int expAmount = 0;
        
        private EPickableState _pickableState = EPickableState.Default;
        private Vector2 startPosition;
        
        private const string PICK_UP_TIMER = "PICK_UP_TIMER";

        private SpriteRenderer spriteRenderer;
        private CircleCollider2D circleCollider2D;

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            spriteRenderer = GetComponent<SpriteRenderer>();
            circleCollider2D = GetComponent<CircleCollider2D>();
            circleCollider2D.radius = range;
            circleCollider2D.isTrigger = true;
        }

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);

            _pickableState = EPickableState.Default;
        }

        public void Setup(EExpGemType gemType, Vector2 position)
        {
            var gemList = PlayerEnchantments.Has(EEnchantmentName.BetterExp)
                ? betterGemAmountPair
                : expAmountPair;
            var pair = gemList.FirstOrDefault(e => e.gemType == gemType);
            if (pair == null) return;

            expAmount = pair.expAmount;
            spriteRenderer.sprite = pair.gemSprite;
            startPosition = position;
            transform.position = startPosition;
            
            OnGet(null);
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            if (_pickableState != EPickableState.PickingUpPhase) return;
            
            var remainingTime = Mathf.Clamp01((float)CheckTimer(PICK_UP_TIMER) / animTime);

            transform.position = Vector3.Lerp(startPosition, PlayerPos, remainingTime);
                    
            if (Vector2.Distance(transform.position, PlayerPos) > 0.1f) return;
                    
            AudioManager.Instance.PlaySound(ESoundType.PickUpGem);

            _pickableState = EPickableState.PickedUp;
                    
            PlayerManager.Instance.PlayerExp.GainExp(expAmount);
            
            ExpPool.Instance.ReleasePoolObject(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_pickableState != EPickableState.Default || !other.CompareTag("Player")) return;
            
            _pickableState = EPickableState.PickingUpPhase;
            SetTimer(PICK_UP_TIMER);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        public enum EPickableState
        {
            Default,
            PickingUpPhase,
            PickedUp,
        }
    }
}