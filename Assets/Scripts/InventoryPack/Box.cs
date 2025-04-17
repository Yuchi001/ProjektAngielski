using System.Collections.Generic;
using System.Linq;
using InventoryPack.WorldItemPack;
using ItemPack.SO;
using Managers;
using Managers.Other;
using UIPack;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventoryPack
{
    public abstract class Box : UIBase
    {
        [SerializeField] private InputActionReference fastActionReference;
        [SerializeField] protected List<BoxGridData> gridDataList;

        protected readonly List<ItemSlot> _itemSlots = new();
        public Canvas CurrentCanvas { get; private set; }

        protected abstract void Awake();

        protected void InitBox(ItemSlot slotPrefab = null)
        {
            DestroySlots();
            CurrentCanvas = GetComponentInParent<Canvas>();
            
            var prefab = slotPrefab ? slotPrefab : GameManager.GetPrefab<ItemSlot>(PrefabNames.ItemSlot);
            SpawnSlots(prefab);
        }

        private void SpawnSlots(ItemSlot prefab)
        {
            var index = 0;
            foreach (var gridData in gridDataList)
            {
                for (var i = 0; i < gridData.Capacity; i++)
                {
                    var spawnedSlot = Instantiate(prefab, gridData.Grid);
                    spawnedSlot.Setup(this, index);
                    _itemSlots.Add(spawnedSlot);
                    index++;
                }
            }
        }

        public virtual bool TryFastInputAction(ItemSlot slot)
        {
            if (fastActionReference.action == null || !fastActionReference.action.IsPressed()) return false;

            var boxes = new List<Box>();
            foreach (var ui in UIManager.GetCurrentUIBaseList())
            {
                var uiScript = ui.Script;
                if (uiScript is not Box box || uiScript == this || !uiScript.isActiveAndEnabled) continue;
                boxes.Add(box);
            }
            if (!boxes.Any()) return false;

            var (item, level) = slot.ViewItem();
            foreach (var box in boxes)
            {
                var result = box.AddItem(item, level);
                if (result == -1) continue;
                
                slot.SetItem(null, -1);
                return true;
            }

            return false;
        }

        public virtual void DropItem(int index)
        {
            var slot = _itemSlots[index];
            var item = slot.ViewItem();
            if (item.item == null) return;
            WorldItemManager.SpawnInventoryItem(item.item, GetItemDropPosition(), item.level);
            slot.SetItem(null, -1);
        }

        protected abstract Vector2 GetItemDropPosition();
        
        /// <summary>
        /// Editor only function
        /// </summary>
        public void SpawnSlots()
        {
            var prefab = Resources.Load<ItemSlot>("Prefabs/UI/ItemSlot");
            var index = 0;
            foreach (var gridData in gridDataList)
            {
                for (var i = 0; i < gridData.Capacity; i++)
                {
                    var spawnedSlot = Instantiate(prefab, gridData.Grid);
                    //spawnedSlot.Setup(this, index);
                    _itemSlots.Add(spawnedSlot);
                    index++;
                }
            }
        }
        
        /// <summary>
        /// Editor only function
        /// </summary>
        public void DestroySlots()
        {
            foreach (var gridTransform in gridDataList.Select(gridData => gridData.Grid.transform))
            {
                for (var i = gridTransform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(gridTransform.GetChild(i).gameObject);
                }
            }
        }

        public void SwitchItems(int current, int target)
        {
            var item1 = _itemSlots[current].ViewItem();
            var item2 = _itemSlots[target].ViewItem();

            AddItemAtSlot(target, item1.item, item1.level);
            AddItemAtSlot(current, item2.item, item2.level);
        }

        public virtual void AddItemAtSlot(int index, SoInventoryItem inventoryItem, int level)
        {
            _itemSlots[index].SetItem(inventoryItem, level);
        }
        
        public virtual int AddItem(SoInventoryItem inventoryItem, int level)
        {
            foreach (var slot in _itemSlots)
            {
                if (slot.TryAddNewItem(inventoryItem, level)) return slot.Index;
            }

            return -1;
        }

        public bool IsFull()
        {
            foreach (var slot in _itemSlots)
            {
                if (slot.IsEmpty() && slot.IsEnabled()) return false;
            }

            return true;
        }
        
        public virtual bool CanAdd(Box fromBox, ItemSlot slot, int targetIndex)
        {
            return true;
        }
        
        /// <summary>
        /// Checks if item can be switched between boxes.
        /// </summary>
        /// <param name="currentInventoryItem">Item in this box to be switched.</param>
        /// <param name="newInventoryItem">Item in other box to be switched.</param>
        /// <returns>true if it is possible to switch items</returns>
        public virtual bool CanSwitch(SoInventoryItem currentInventoryItem, SoInventoryItem newInventoryItem)
        {
            return true;
        }

        public virtual void RemoveItemAtSlot(int index)
        {
            _itemSlots[index].SetItem(null, -1);
        }

        public virtual void OnSlotChanged(ItemSlot itemSlot)
        {
            
        }

        public ItemSlot GetSlotAtIndex(int index)
        {
            return _itemSlots[index];
        }

        public virtual bool CanInteract()
        {
            return true;
        }

        [System.Serializable]
        public struct BoxGridData
        {
            [SerializeField] private int capacity;
            [SerializeField] private Transform grid;

            public int Capacity => capacity;
            public Transform Grid => grid;
        }
    }
}
