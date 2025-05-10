using System;
using ItemPack.SO;
using Other;
using PoolPack;
using UnityEngine;
using Utils;

namespace InventoryPack.WorldItemPack
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class WorldItem : PoolObject
    {
        [SerializeField] private float pickUpDistance = 0.5f;
        [SerializeField] private float pickUpTime = 0.2f;
        [SerializeField] private MinMax throwSpeed;
        [SerializeField] private float throwSlowAcceleration = 3f;
        [SerializeField] private float pickUpCooldown = 0.75f;
        [SerializeField] private float itemLifeTime = 20;
        
        private SpriteRenderer _spriteRenderer;
        public Animator Anim { get; private set; }
        public SoItem Item { get; private set; }

        private PoolManager _poolManager;

        private int[] _paramArray = Array.Empty<int>();

        private Vector2 _pickUpStartPos;

        private LiveState _liveState;
        private PickUpState _pickUpState;
        private ThrowState _throwState;
        private ReleaseState _releaseState;

        private IWorldItemState _currentState;
        
        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _poolManager = poolManager;
            Anim = GetComponent<Animator>();
            Anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
            
            _liveState = new LiveState(pickUpDistance, itemLifeTime, _poolManager, () => _pickUpState);
            _pickUpState = new PickUpState(pickUpTime, () => _liveState, () => _releaseState);
            _throwState = new ThrowState(throwSpeed, throwSlowAcceleration, () => _liveState);
            _releaseState = new ReleaseState(0.15f, _poolManager);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            _currentState = null;
            Item = null;
            _spriteRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            _currentState = null;
        }

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);

            _spriteRenderer.enabled = true;
            _paramArray = Array.Empty<int>();
            
            SwitchState(_throwState);
        }

        public void Setup(SoItem item, Vector2 position, params int[] paramArray)
        {
            OnGet(null);
            transform.position = position;
            Item = item;
            _paramArray = paramArray;
            _spriteRenderer.sprite = item.ItemSprite;
        }
        
        public bool TryPickUp() => Item.OnPickUp(_paramArray);

        public void SwitchState(IWorldItemState newState)
        {
            _currentState = newState;
            _currentState.Enter(this);
        }

        private void Update()
        {
            _currentState?.Execute(this);
        }

        protected override void LazyUpdate(float lazyDeltaTime)
        {
            _currentState?.LazyExecute(this, lazyDeltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, pickUpDistance);
        }
    }
}