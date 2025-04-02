using System;
using System.Collections;
using ItemPack.Enums;
using ItemPack.SO;
using Other;
using PlayerPack;
using PlayerPack.Enums;
using PoolPack;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;
using Random = UnityEngine.Random;

namespace InventoryPack.WorldItemPack
{
    [RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class WorldItem : PoolObject
    {
        [SerializeField] private float pickUpDistance = 0.5f;
        [SerializeField] private float getDistance = 0.1f;
        [SerializeField] private float movementSpeed = 3f;
        [SerializeField] private float movementAcceleration = 1f;
        [SerializeField] private float pickUpCooldown = 0.75f;
        
        private SpriteRenderer _spriteRenderer;
        private Animator _anim;
        private SoItem _item;
        private bool _canPickUp = false;

        private PoolManager _poolManager;

        private int[] _paramArray = Array.Empty<int>();

        private float _currentMovementSpeed;

        private bool _chasePlayer = false;
        private bool _pickedUp = false;
        private bool _cleanUp = false;

        private float _currentSpeed;
        private bool _ready = false;

        private Vector2 _randomDir;
        
        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _poolManager = poolManager;
            _anim = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            _item = null;
            _canPickUp = false;
            _spriteRenderer.enabled = false;
        }

        public override void OnGet(SoPoolObject so)
        {
            base.OnGet(so);

            _ready = false;
            _currentSpeed = movementSpeed;
            _spriteRenderer.enabled = true;
            _chasePlayer = false;
            _pickedUp = false;
            _cleanUp = false;
            _currentMovementSpeed = movementSpeed;
            _paramArray = Array.Empty<int>();
        }

        public void Setup(SoItem item, Vector2 position, params int[] paramArray)
        {
            Ready();
            transform.position = position;
            _item = item;
            _paramArray = paramArray;
            _spriteRenderer.sprite = item.ItemSprite;
        }

        private void Ready()
        {
            OnGet(null);

            _randomDir = new Vector2
            {
                x = Random.Range(-1f, 1f),
                y = Random.Range(-1f, 1f),
            }.normalized;
            _ready = true;
            StartCoroutine(SetCanPickUp());
        }

        private IEnumerator SetCanPickUp()
        {
            yield return new WaitForSeconds(pickUpCooldown);

            _canPickUp = true;
        }

        public override void InvokeUpdate()
        {
            if (!_ready) return;

            if (_currentSpeed > 0 && !_canPickUp)
            {
                transform.Translate(_randomDir * _currentSpeed);
                _currentSpeed -= movementSpeed * Time.deltaTime;
                return;
            }
            
            if (!_canPickUp) return;

            var playerPos = PlayerManager.Instance.PlayerPos;
            if (_cleanUp)
            {
                transform.position = playerPos;
                return;
            }

            var dist = transform.Distance(playerPos);
            if (dist > pickUpDistance && !_pickedUp || !_item.CanPickUp()) return;

            if (_pickedUp == false) StartCoroutine(PickUp());
            if (_chasePlayer == false) return;

            _currentMovementSpeed += movementAcceleration * Time.deltaTime;
            transform.MoveTowards(playerPos, _currentMovementSpeed);
            if (dist >= getDistance) return;

            var pickedUp = _item.OnPickUp(_paramArray);
            if (!pickedUp)
            {
                _pickedUp = false;
                _chasePlayer = false;
                return;
            }

            StartCoroutine(Release());
        }

        private IEnumerator PickUp()
        {
            _anim.SetTrigger("PickUp");
            _pickedUp = true;
            yield return new WaitForSeconds(0.2f);
            _chasePlayer = true;
        }

        private IEnumerator Release()
        {
            _cleanUp = true;
            _anim.SetTrigger("Exit");
            yield return new WaitForSeconds(0.15f);
            _poolManager.ReleasePoolObject(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, pickUpDistance);
            Gizmos.DrawWireSphere(transform.position, getDistance);
        }
    }
}