 using System.Collections;
 using System.Collections.Generic;
using System.Linq;
using InventoryPack;
using InventoryPack.WorldItemPack;
using ItemPack;
using ItemPack.SO;
using PlayerPack.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerItemManager : Box
    {
        [SerializeField] private int functionalSlotIndexEnd;
        
        private readonly List<ItemLogicBase> _currentItems = new();
        private List<SoInventoryItem> _allItems = new();
        private List<(float weight, SoInventoryItem item)> _normalizedWeightItemList = new();
        private float weightSum = 0;

        public delegate void ItemAddDelegate(ItemLogicBase itemLogicBase);
        public static event ItemAddDelegate OnItemAdd;

        private static int CAPACITY => PlayerManager.PlayerStatsManager.GetStatAsInt(EPlayerStatType.Capacity);
        private GameObject ITEMS_GRID => gridDataList[0].Grid.gameObject;
        private GameObject BACKPACK_GRID => gridDataList[1].Grid.gameObject;

        private bool _canInteract;

        protected override void Init()
        {
            _allItems = Resources.LoadAll<SoInventoryItem>("InventoryItems").Select(Instantiate).ToList();

            var weightItemList = _allItems.Select(item => (weight: 1f / item.ItemPrice, item: item)).ToList();
            weightSum = weightItemList.Sum(w => w.weight);
            _normalizedWeightItemList = weightItemList.Select(pair => (weight: pair.weight / weightSum, item: pair.item)).ToList();

            _canInteract = true;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.I)) return;

            ToggleEq(!_canInteract);
        }

        public void ToggleEq(bool open)
        {
            _canInteract = open;
        }

        public override bool CanInteract()
        {
            return _canInteract;
        }

        private void AddItemLogic(SoInventoryItem inventoryItemToAdd, int level)
        {
            if (PlayerManager.CurrentState != PlayerManager.State.ON_MISSION) return;
            
            var itemLogic = Instantiate(inventoryItemToAdd.ItemPrefab);
            itemLogic.transform.localPosition = Vector3.zero;
            itemLogic.Setup(inventoryItemToAdd, level);
            _currentItems.Add(itemLogic);

            OnItemAdd?.Invoke(itemLogic);
        }

        public override int AddItem(SoInventoryItem inventoryItem, int level)
        {
            var index = base.AddItem(inventoryItem, level);
            if (index == -1) return -1;
            
            if (index < functionalSlotIndexEnd) AddItemLogic(inventoryItem, level);
            return index;
        }

        public void CleanInventory()
        {
            DestroyAllItems();
            foreach (var slot in _itemSlots)
            {
                slot.SetItem(null, -1);
            }
        }

        public void RefreshInventory()
        {
            DestroyAllItems();
            foreach (var slot in _itemSlots)
            {
                if (slot.Index >= functionalSlotIndexEnd) return; 
                var itemPair = slot.ViewItem();
                if (itemPair.item == null) continue;
                AddItemLogic(itemPair.item, itemPair.level);
            }
        }

        public override void AddItemAtSlot(int index, SoInventoryItem inventoryItem, int level)
        {
            base.AddItemAtSlot(index, inventoryItem, level);
            RefreshInventory();
        }

        public override void RemoveItemAtSlot(int index)
        {
            base.RemoveItemAtSlot(index);
            RefreshInventory();
        }
        
        public void RemoveItemIntoWorldAtSlot(int index)
        {
            var current = _itemSlots[index].ViewItem();
            WorldItemManager.SpawnInventoryItem(Instantiate(current.item), PlayerManager.PlayerPos, current.level);
            RemoveItemAtSlot(index);
        }

        public IEnumerable<SoInventoryItem> GetRandomItems(int count, float percentage = 0.0f)
        {
            var selectedItems = new List<SoInventoryItem>();

            for (var i = 0; i < count; i++)
            {
                var roll = Random.value;

                roll -= Random.value * percentage;
                roll = Mathf.Max(roll, 0f);

                var cumulativeWeight = 0f;

                foreach (var entry in _normalizedWeightItemList)
                {
                    cumulativeWeight += entry.weight;

                    if (roll > cumulativeWeight) continue;
                    
                    selectedItems.Add(entry.item);
                    break;
                }
            }

            return selectedItems;
        }

        public void DestroyAllItems()
        {
            foreach (var item in _currentItems.Where(i => i != null))
            {
                Destroy(item.gameObject);
            }
            _currentItems.Clear();
        }
    }
}