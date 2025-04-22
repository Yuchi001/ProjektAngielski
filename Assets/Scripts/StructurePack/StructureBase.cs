using System;
using DamageIndicatorPack;
using Managers;
using PlayerPack;
using StructurePack.SO;
using TMPro;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WorldGenerationPack;

namespace StructurePack
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class StructureBase : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bottomInteractionMessageField;
        [SerializeField] private SpriteRenderer structureSpriteRenderer;
        [SerializeField] private float collisionRange = 2;
        [SerializeField] private string uiPrefabName;
        
        private SoStructure _structureData;
        public SoStructure StructureData => _structureData;
        private CircleCollider2D Collider => GetComponent<CircleCollider2D>();


        public bool Toggle { get; private set; } = false;
        public bool WasUsed { get; private set; } = false;
        public float OffsetY { get; private set; }

        private Light2D _spriteLight;

        private bool _isFocused = false;

        private IOpenStrategy _openStrategy;
        private ICloseStrategy _closeStrategy;

        private int _interactionCount;

        public bool CanInteract { get; private set; } = true;
        
        private object _data;
        
        private void Awake()
        {
            Collider.isTrigger = true;
        }

        public StructureBase Setup(SoStructure structureData)
        {
            var spriteTransform = structureSpriteRenderer.transform;
            _spriteLight = structureSpriteRenderer.GetComponent<Light2D>();
            
            Collider.radius = collisionRange * structureData.StructureScale;
            
            _structureData = structureData;
            var basicSprite = _structureData.GetSprite(SoStructure.EState.NOT_USED);
            structureSpriteRenderer.sprite = basicSprite;
            spriteTransform.localScale *= structureData.StructureScale;

            _spriteLight.lightCookieSprite = basicSprite;

            var height = basicSprite.rect.height;
            OffsetY = 0.5f * (height / 16.0f) * structureData.StructureScale * (16f / basicSprite.pixelsPerUnit);
            Collider.offset = new Vector2(0, OffsetY);

            var newPos = new Vector3(0, OffsetY);
            
            spriteTransform.localPosition = newPos;
            
            bottomInteractionMessageField.gameObject.SetActive(false);
            
            _structureData.OnSetup(this);

            _openStrategy = _structureData.GetOpenStrategy(this);
            _closeStrategy = _structureData.GetCloseStrategy();

            return this;
        }

        public IOpenStrategy CurrentOpenStrategy() => _openStrategy;
        public ICloseStrategy CurrentCloseStrategy() => _closeStrategy;

        public void HandleInteraction()
        {
            if (!CanInteract) return;
            
            var success = _structureData.OnInteract(this);
            if (!success)
            {
                IndicatorManager.SpawnIndicator(transform.position, _structureData.InteractionDeclineMessage, Color.red);
                return;
            }

            if (!_structureData.Reusable || (_interactionCount >= _structureData.InteractionLimit && _interactionCount > 0))
            {
                structureSpriteRenderer.sprite = _structureData.GetSprite(SoStructure.EState.USED);
                _spriteLight.enabled = false;
                bottomInteractionMessageField.gameObject.SetActive(false);
                StructureManager.RemoveFromQueue(this);
            }
            else structureSpriteRenderer.sprite = _structureData.GetSprite(SoStructure.EState.ACTIVE);

            _interactionCount++;
            bottomInteractionMessageField.text = _structureData.GetInteractionMessage(this);
            Toggle = !Toggle;
            WasUsed = true;
        }

        public void SetCanInteract(bool canInteract)
        {
            CanInteract = canInteract;
        }

        public T GetData<T>() where T: class, new()
        {
            if (_data == null) _data = new T();
            return _data as T;
        }

        public void SetData<T>(T data, bool notify = true) where T: class, new()
        {
            _data = data;
            if (notify) _structureData.OnDataChange(data);
        }

        private void Update()
        {
            var isFocused = StructureManager.IsFocused(this);
            if (_isFocused != isFocused) OnFocusChanged(isFocused);
        }

        private void OnFocusChanged(bool newValue)
        {
            _isFocused = newValue;
            if (!bottomInteractionMessageField.gameObject.activeSelf)
            {
                var message = _structureData.GetInteractionMessage(this);
                bottomInteractionMessageField.text = message == "" ? "Press E" : message;
            }
            bottomInteractionMessageField.gameObject.SetActive(newValue);
            _structureData.OnFocusChanged(this, newValue);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || (WasUsed && !_structureData.Reusable) || !CanInteract) return;
            
            StructureManager.AddToQueue(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            bottomInteractionMessageField.gameObject.SetActive(false);
            StructureManager.RemoveFromQueue(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, collisionRange);
        }

        private void OnDisable()
        {
            StructureManager.RemoveFromQueue(this);
        }
    }
}