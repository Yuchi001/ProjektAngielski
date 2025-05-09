﻿using InventoryPack.WorldItemPack;
using ItemPack.SO;
using Managers;
using Managers.Other;
using PlayerPack;
using TMPro;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventoryPack
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        protected SoInventoryItem _current = null;
        private DragItemSlot _dragItemSlot;
        private Image _itemImage;
        private Image _backgroundImage;
        protected int _level;
        [SerializeField] protected Color dragItemColor;
        [SerializeField] protected Color defaultItemColor;
        [SerializeField] protected Color disabledItemColor;
        private bool _enabled = true;
        private Box _box;
        private ItemInformationHover _informationPrefab;
        private ItemInformationHover _spawnedInformationInstance = null;
        private bool _drag = false;
        
        private string INFORMATION_UI_KEY => $"InformationHover{GetInstanceID()}";
        private IOpenStrategy _informationOpenStrategy;
        private ICloseStrategy _informationCloseStrategy;

        private TextMeshProUGUI _levelText;
        
        public int Index { get; private set; }

        public void Setup(Box box, int index, bool enabled = true)
        {
            _box = box;
            _enabled = enabled;
            _dragItemSlot = transform.GetComponentInChildren<DragItemSlot>();
            _dragItemSlot.Setup(this, box);
            _itemImage = transform.GetChild(2).GetComponent<Image>();
            _backgroundImage = transform.GetChild(1).GetComponent<Image>();
            _levelText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            Index = index;
            _itemImage.color = Color.clear;
            _backgroundImage.color = enabled ? defaultItemColor : disabledItemColor;
            _levelText.text = "";
            var informationPrefab = GameManager.GetPrefab<ItemInformationHover>(PrefabNames.ItemInformationUI);
            _informationOpenStrategy = new CloseAllOfTypeOpenStrategy<ItemInformationHover>(informationPrefab, true);
            _informationCloseStrategy = new DestroyCloseStrategy(INFORMATION_UI_KEY);
            if (!enabled) SetItem(null, -1);
        }

        public bool TryAddNewItem(SoInventoryItem inventoryItem, int level)
        {
            if (_current != null || inventoryItem == null || !_enabled) return false;

            SetItem(inventoryItem, level);
            return true;
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
            _backgroundImage.color = enabled ? defaultItemColor : disabledItemColor;
            if (!enabled) SetItem(null, -1);
        }

        public virtual void SetItem(SoInventoryItem inventoryItem, int level, bool sendNotification = true)
        {
            _level = level;
            _current = inventoryItem ? Instantiate(inventoryItem) : null;
            _itemImage.sprite = _current ? _current.ItemSprite : null;
            _itemImage.color = _current ? defaultItemColor : Color.clear;
            _levelText.text = inventoryItem == null ? "" : level.ToString();
            
            if (sendNotification) _box.OnSlotChanged(this);
        }

        public (SoInventoryItem item, int level) ViewItem()
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

            if (_box.TryFastInputAction(this)) return;
            
            OnDragStart(eventData);
        }

        protected virtual void OnDragStart(PointerEventData eventData)
        {
            _dragItemSlot.BeginDrag();
            _itemImage.color = dragItemColor;
            _drag = true;
            OnPointerExit(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsEmpty() || !_enabled || !_box.CanInteract()) return;
            
            OnDragEnd(eventData);
        }

        protected virtual void OnDragEnd(PointerEventData eventData)
        {
            var rayCastHitGO = eventData.pointerCurrentRaycast.gameObject;
            if (rayCastHitGO && rayCastHitGO.TryGetComponent(out ItemSlot itemSlot) && itemSlot.IsEnabled())
                _dragItemSlot.EndDrag(itemSlot.Index, itemSlot.GetComponentInParent<Box>());
            else _dragItemSlot.EndDrag();
            
            _levelText.text = _current ? _level.ToString() : "";
            _itemImage.color = _current ? defaultItemColor : Color.clear;
            _drag = false;

            if (rayCastHitGO == null) _box.DropItem(Index);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty() || !_enabled || _drag || !_box.CanInteract() || _spawnedInformationInstance != null) return;

            _spawnedInformationInstance = UIManager.OpenUI<ItemInformationHover>(INFORMATION_UI_KEY, _informationOpenStrategy, _informationCloseStrategy);
            _spawnedInformationInstance.Setup(_current, _level, _box);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spawnedInformationInstance == null) return;
            
            Destroy(_spawnedInformationInstance.gameObject);
        }
    }
}