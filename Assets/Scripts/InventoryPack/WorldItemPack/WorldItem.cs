using System;
using System.Collections;
using ItemPack.SO;
using Other;
using PlayerPack;
using PoolPack;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;
using Random = UnityEngine.Random;

namespace InventoryPack.WorldItemPack
{
    [RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class WorldItem : PoolObject
    {
        [SerializeField] private float forceMagnitude = 0.5f;
        [SerializeField] private float pickUpDistance = 0.5f;
        [SerializeField] private float getDistance = 0.1f;
        [SerializeField] private float movementSpeed = 3f;
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        private Light2D _light2D;
        private Animator _anim;
        private SoItem _item;
        private int _level;
        private bool _isCoin = false;
        private bool _canPickUp = false;

        private PoolManager _poolManager;

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _poolManager = poolManager;
            _anim = GetComponent<Animator>();
            _light2D = GetComponent<Light2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer.enabled = false;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            _item = null;
            _isCoin = false;
            _level = 1;
            _canPickUp = false;
            _spriteRenderer.enabled = false;
            _light2D.enabled = false;
        }

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);
            
            _spriteRenderer.enabled = true;
            _light2D.enabled = true;
        }

        public void Setup(SoItem item, int level, Vector2 position)
        {
            transform.position = position;
            _item = item;
            _level = level;
            _isCoin = false;
            _spriteRenderer.sprite = item.ItemSprite;
            _light2D.lightCookieSprite = item.ItemSprite;
            
            Ready();
        }
        
        public void Setup(Sprite coinSprite, int value, Vector2 position)
        {
            transform.position = position;
            _spriteRenderer.sprite = coinSprite;
            _level = value;
            _isCoin = true;
            _light2D.lightCookieSprite = coinSprite;
            
            Ready();
        }

        private void Ready()
        {
            OnGet(null);

            var randomDir = new Vector2
            {
                x = Random.Range(-1f, 1f),
                y = Random.Range(-1f, 1f),
            }.normalized;
            _rigidbody2D.AddForce(randomDir * forceMagnitude, ForceMode2D.Impulse);
            StartCoroutine(SetCanPickUp());
        }

        private IEnumerator SetCanPickUp()
        {
            yield return new WaitForSeconds(0.5f);

            _canPickUp = true;
        }

        public override void InvokeUpdate()
        {
            if (PlayerManager.Instance.PlayerItemManager.IsFull() && !_isCoin || !_canPickUp) return;
            
            var playerPos = PlayerManager.Instance.PlayerPos;
            var dist = transform.Distance(playerPos);
            if (dist >= pickUpDistance) return;

            transform.MoveTowards(playerPos, movementSpeed);
            if (dist >= getDistance) return;

            if (_isCoin)
            {
                PlayerManager.Instance.PlayerItemManager.AddCoins(_level); 
                _poolManager.ReleasePoolObject(this);
                return;
            }
            
            var index = PlayerManager.Instance.PlayerItemManager.AddItem(Instantiate(_item), _level);
            if (index != -1) _poolManager.ReleasePoolObject(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, pickUpDistance);
            Gizmos.DrawWireSphere(transform.position, getDistance);
        }
    }
}