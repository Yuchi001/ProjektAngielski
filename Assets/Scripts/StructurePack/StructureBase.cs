using Managers;
using Managers.Other;
using StructurePack.InteractionUI;
using StructurePack.SO;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace StructurePack
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class StructureBase : MonoBehaviour
    {
        [SerializeField] private GameObject interactionMessage;
        [SerializeField] private SpriteRenderer structureSpriteRenderer;
        [SerializeField] private float collisionRange = 2;
        
        private SoStructure _structureData;
        private CircleCollider2D Collider => GetComponent<CircleCollider2D>();

        private bool _inRange = false;

        public bool Toggle { get; private set; } = false;
        public bool WasUsed { get; private set; } = false;

        private Light2D _spriteLight;

        private IOpenStrategy _openStrategy;
        private ICloseStrategy _closeStrategy;
        
        private void Awake()
        {
            Collider.isTrigger = true;
            var prefab = GameManager.Instance.GetPrefab<TotemInteractionUI>(PrefabNames.TotemInteractionUI);
            _openStrategy = new CloseAllOfTypeOpenStrategy<IStructure>(prefab, false);
            _closeStrategy = new DefaultCloseStrategy();
        }

        public void Setup(SoStructure structureData)
        {
            var spriteTransform = structureSpriteRenderer.transform;
            _spriteLight = structureSpriteRenderer.GetComponent<Light2D>();
            
            Collider.radius = collisionRange * structureData.StructureScale;
            
            _structureData = structureData;
            structureSpriteRenderer.sprite = structureData.StructureSprite;
            spriteTransform.localScale *= structureData.StructureScale;

            _spriteLight.lightCookieSprite = structureData.StructureSprite;

            var height = structureData.StructureSprite.rect.height;
            var offsetY = 0.5f * (height / 16.0f) * structureData.StructureScale;

            var newPos = new Vector3(0, offsetY);
            
            spriteTransform.localPosition = newPos;
            
            interactionMessage.SetActive(false);
        }

        private void Update()
        {
            if (!_inRange || (WasUsed && !_structureData.Reusable) || !Input.GetKeyDown(KeyCode.E)) return;

            if (!_structureData.Reusable)
            {
                structureSpriteRenderer.sprite = _structureData.UsedStructureSprite;
                _spriteLight.enabled = false;
                interactionMessage.SetActive(false);
            }
            
            HandleInteraction();
        }

        public void HandleInteraction()
        {
            Toggle = !Toggle;
            _structureData.OnInteract(this, _openStrategy, _closeStrategy);
            WasUsed = true;
        }

        public void HandleCloseUI()
        {
            Time.timeScale = 1;
            Toggle = false;
            WasUsed = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || (WasUsed && !_structureData.Reusable)) return;

            _inRange = true;
            interactionMessage.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = false;
            interactionMessage.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, collisionRange);
        }
    }
}