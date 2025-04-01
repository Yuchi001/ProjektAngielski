using DamageIndicatorPack;
using Managers;
using StructurePack.SO;
using TMPro;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        private CircleCollider2D Collider => GetComponent<CircleCollider2D>();

        private bool _inRange = false;

        public bool Toggle { get; private set; } = false;
        public bool WasUsed { get; private set; } = false;

        private Light2D _spriteLight;

        private IOpenStrategy _openStrategy;
        private ICloseStrategy _closeStrategy;

        private int _interactionCount;
        
        private object _data;
        
        private void Awake()
        {
            Collider.isTrigger = true;
            var prefab = GameManager.GetPrefab<UIBase>(uiPrefabName);
            _openStrategy = new CloseAllOfTypeOpenStrategy<IStructure>(prefab, false);
            _closeStrategy = new DefaultCloseStrategy();
        }

        public void Setup(SoStructure structureData)
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
            var offsetY = 0.5f * (height / 16.0f) * structureData.StructureScale;

            var newPos = new Vector3(0, offsetY);
            
            spriteTransform.localPosition = newPos;
            
            bottomInteractionMessageField.gameObject.SetActive(false);
            
            _structureData.OnSetup(this);
        }

        private void Update()
        {
            if (!_inRange || (WasUsed && !_structureData.Reusable) || !Input.GetKeyDown(KeyCode.E)) return;

            HandleInteraction();
        }

        public void HandleInteraction()
        {
            var success = _structureData.OnInteract(this, _openStrategy, _closeStrategy);
            if (!success)
            {
                IndicatorManager.SpawnIndicator(transform.position, _structureData.InteractionDeclineMessage, Color.red);
                return;
            }

            if (!_structureData.Reusable || _interactionCount >= _structureData.InteractionLimit)
            {
                structureSpriteRenderer.sprite = _structureData.GetSprite(SoStructure.EState.USED);
                _spriteLight.enabled = false;
                bottomInteractionMessageField.gameObject.SetActive(false);
            }
            else structureSpriteRenderer.sprite = _structureData.GetSprite(SoStructure.EState.ACTIVE);

            _interactionCount++;
            bottomInteractionMessageField.text = _structureData.GetInteractionMessage(this);
            Toggle = !Toggle;
            WasUsed = true;
        }

        public void HandleCloseUI()
        {
            Time.timeScale = 1;
            Toggle = false;
            WasUsed = true;
        }

        public T GetData<T>() where T: class, new()
        {
            if (_data == null) _data = new T();
            return _data as T;
        }

        public void SetData(object data)
        {
            _data = data;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || (WasUsed && !_structureData.Reusable)) return;

            _inRange = true;
            var message = _structureData.GetInteractionMessage(this);
            bottomInteractionMessageField.text = message == "" ? "Press E" : message;
            bottomInteractionMessageField.gameObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = false;
            bottomInteractionMessageField.gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, collisionRange);
        }
    }
}