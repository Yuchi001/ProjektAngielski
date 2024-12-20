using ItemPack.SO;
using Managers;
using Managers.Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventoryPack
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private SoItem _current = null;
        private DragItemSlot _dragItemSlot;
        private Image _itemImage;
        private Image _backgroundImage;
        private int _level;
        private static readonly Color _dragColor = new(0.5f, 0.5f, 0.5f, 0.3f);
        private static readonly Color _tonedColor = new(0.5f, 0.5f, 0.5f, 1);
        private static readonly Color _disabledColor = new(0.35f, 0.35f, 0.35f, 1);
        private bool _enabled = true;
        private Box _box;
        private ItemInformationHover _informationPrefab;
        private ItemInformationHover _spawnedInformationInstance = null;
        private bool _drag = false;
        
        public int Index { get; private set; }

        public void Setup(Box box, int index, bool enabled = true)
        {
            _box = box;
            _enabled = enabled;
            _dragItemSlot = transform.GetComponentInChildren<DragItemSlot>();
            _dragItemSlot.Setup(this, box);
            _itemImage = transform.GetChild(transform.childCount - 1).GetComponent<Image>();
            _backgroundImage = transform.GetChild(1).GetComponent<Image>();
            Index = index;
            _itemImage.color = Color.clear;
            _backgroundImage.color = enabled ? _tonedColor : _disabledColor;
            _informationPrefab = GameManager.Instance.GetPrefab<ItemInformationHover>(PrefabNames.ItemInformationUI);
            if (!enabled) SetItem(null, -1);
        }

        public bool TryAddNewItem(SoItem item, int level)
        {
            if (_current || !item) return false;

            SetItem(item, level);
            return true;
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
            _backgroundImage.color = enabled ? _tonedColor : _disabledColor;
            if (!enabled) SetItem(null, -1);
        }

        public void SetItem(SoItem item, int level)
        {
            _level = level;
            _current = item ? Instantiate(item) : null;
            _itemImage.sprite = _current ? _current.ItemSprite : null;
            _itemImage.color = _current ? _tonedColor : Color.clear;
        }

        public (SoItem item, int level) ViewItem()
        {
            return (item: _current, level: _level);
        }

        public bool IsEmpty()
        {
            return _current == null;
        }

        public bool IsEnabled()
        {
            return _enabled;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_current == null || !enabled || !_box.CanInteract()) return;
            
            _dragItemSlot.BeginDrag();
            _itemImage.color = _dragColor;
            _drag = true;
            OnPointerExit(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsEmpty() || !_enabled || !_box.CanInteract()) return;
            
            var rayCastHitGO = eventData.pointerCurrentRaycast.gameObject;
            if (rayCastHitGO && rayCastHitGO.TryGetComponent(out ItemSlot itemSlot) && itemSlot.IsEnabled())
                _dragItemSlot.EndDrag(itemSlot.Index, itemSlot.GetComponentInParent<Box>());
            else _dragItemSlot.EndDrag();
            
            _itemImage.color = _current ? _tonedColor : Color.clear;
            _drag = false;
            //OnPointerEnter(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty() || !_enabled || _drag || !_box.CanInteract() || _spawnedInformationInstance != null) return;

            _spawnedInformationInstance = Instantiate(_informationPrefab, GameUiManager.Instance.GameCanvas);
            _spawnedInformationInstance.Setup(_current, _level, _box);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spawnedInformationInstance == null) return;
            
            Destroy(_spawnedInformationInstance.gameObject);
        }
    }
}