using System.Collections.Generic;
using ItemPack.SO;
using Managers;
using Managers.Other;
using UnityEngine;

namespace InventoryPack
{
    public abstract class Box : MonoBehaviour
    {
        [SerializeField] protected List<BoxGridData> gridDataList;

        protected readonly List<ItemSlot> _itemSlots = new();
        public Canvas CurrentCanvas { get; private set; }

        protected bool _opened = false;

        private void Awake()
        {
            CurrentCanvas = GetComponentInParent<Canvas>();
            
            var prefab = GameManager.Instance.GetPrefab<ItemSlot>(PrefabNames.ItemSlot);
            SpawnSlots(prefab);
            Init();
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
                    spawnedSlot.Setup(this, index);
                    _itemSlots.Add(spawnedSlot);
                    index++;
                }
            }
        }

        protected abstract void Init();

        public void SwitchItems(int current, int target)
        {
            var item1 = _itemSlots[current].ViewItem();
            var item2 = _itemSlots[target].ViewItem();

            AddItemAtSlot(target, item1.item, item1.level);
            AddItemAtSlot(current, item2.item, item2.level);
        }

        public virtual void AddItemAtSlot(int index, SoItem item, int level)
        {
            _itemSlots[index].SetItem(item, level);
        }
        
        public virtual int AddItem(SoItem item, int level)
        {
            foreach (var slot in _itemSlots)
            {
                if (slot.TryAddNewItem(item, level)) return slot.Index;
            }

            return -1;
        }
        
        /// <summary>
        /// Checks if item can be added to box.
        /// </summary>
        /// <param name="item">New item to be added.</param>
        /// <returns>true if it is possible to add and item</returns>
        public virtual bool CanAdd(SoItem item)
        {
            return true;
        }
        
        /// <summary>
        /// Checks if item can be switched between boxes.
        /// </summary>
        /// <param name="currentItem">Item in this box to be switched.</param>
        /// <param name="newItem">Item in other box to be switched.</param>
        /// <returns>true if it is possible to switch items</returns>
        public virtual bool CanSwitch(SoItem currentItem, SoItem newItem)
        {
            return true;
        }

        public virtual void RemoveItemAtSlot(int index)
        {
            _itemSlots[index].SetItem(null, -1);
        }

        public ItemSlot GetSlotAtIndex(int index)
        {
            return _itemSlots[index];
        }

        public virtual bool CanInteract()
        {
            return true;
        }

        public virtual void Open()
        {
            _opened = true;
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            _opened = false;
            gameObject.SetActive(false);
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
