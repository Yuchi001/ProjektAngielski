using System;
using StructurePack.SO;
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

        private bool _toggle = false;
        private bool _wasUsed = false;

        public bool Toggle => _toggle;
        public bool WasUsed => _wasUsed;
        
        private void Awake()
        {
            Collider.isTrigger = true;
        }

        public void Setup(SoStructure structureData)
        {
            var spriteTransform = structureSpriteRenderer.transform;
            var spriteLight = structureSpriteRenderer.GetComponent<Light2D>();
            
            Collider.radius = collisionRange * structureData.StructureScale;
            
            _structureData = structureData;
            structureSpriteRenderer.sprite = structureData.StructureSprite;
            spriteTransform.localScale *= structureData.StructureScale;

            spriteLight.lightCookieSprite = structureData.StructureSprite;

            var height = structureData.StructureSprite.rect.height;
            var offsetY = 0.5f * (height / 16.0f) * structureData.StructureScale;

            var newPos = new Vector3(0, offsetY);
            
            spriteTransform.localPosition = newPos;
            
            interactionMessage.SetActive(false);
        }

        private void Update()
        {
            if (!_inRange || (_wasUsed && !_structureData.Reusable) || _toggle || !Input.GetKeyDown(KeyCode.E)) return;

            _wasUsed = true;
            _toggle = !_toggle;
            _structureData.OnInteract(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

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