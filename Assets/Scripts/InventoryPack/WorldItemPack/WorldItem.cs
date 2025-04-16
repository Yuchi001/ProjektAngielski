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
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class WorldItem : PoolObject
    {
        [SerializeField] private float pickUpDistance = 0.5f;
        [SerializeField] private float pickUpTime = 0.2f;
        [SerializeField] private MinMax throwSpeed;
        [SerializeField] private float throwSlowAcceleration = 3f;
        [SerializeField] private float pickUpCooldown = 0.75f;
        [SerializeField] private float lifeTime;
        
        private SpriteRenderer _spriteRenderer;
        private Animator _anim;
        private SoItem _item;
        private bool _canPickUp = false;

        private float _lifeTimeTimer = 0;

        private PoolManager _poolManager;

        private int[] _paramArray = Array.Empty<int>();
        
        private bool _chasePlayer = false;
        private bool _pickedUp = false;
        private bool _cleanUp = false;

        private float _pickUpTimer = 0;

        private float _currentThrowSpeed;
        private bool _ready = false;

        private Vector2 _randomDir;

        private Vector2 _pickUpStartPos;
        
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
            _currentThrowSpeed = throwSpeed.RandomFloat();
            _spriteRenderer.enabled = true;
            _chasePlayer = false;
            _pickedUp = false;
            _cleanUp = false;
            _pickUpTimer = 0;
            _paramArray = Array.Empty<int>();
            _lifeTimeTimer = 0;
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
            base.InvokeUpdate();
            
            if (!_ready) return;

            if (_currentThrowSpeed > 0 && !_canPickUp)
            {
                _currentThrowSpeed -= throwSlowAcceleration * deltaTime;
                transform.Translate(_randomDir * (_currentThrowSpeed * deltaTime));
                return;
            }
            
            if (!_canPickUp) return;

            var playerPos = PlayerManager.PlayerPos;
            if (_cleanUp)
            {
                transform.position = playerPos;
                return;
            }

            var dist = transform.Distance(playerPos);
            if (dist > pickUpDistance && !_pickedUp || !_item.CanPickUp())
            {
                _lifeTimeTimer += deltaTime;
                if (_lifeTimeTimer >= lifeTime)
                {
                    //TODO: jakis indikator ze item zniknie
                    _poolManager.ReleasePoolObject(this);
                }
                return;
            }

            if (_pickedUp == false) StartCoroutine(PickUp());
            if (_chasePlayer == false) return;

            _pickUpTimer += deltaTime;
            var currentTime = Mathf.Clamp01(_pickUpTimer / pickUpTime);
            transform.position = Vector2.Lerp(_pickUpStartPos, PlayerManager.PlayerPos, currentTime);
            if (currentTime < 1f) return;

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
            _pickUpStartPos = transform.position;
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
        }
    }
}