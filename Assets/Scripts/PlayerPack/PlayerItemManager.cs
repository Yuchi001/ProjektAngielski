 using System;
 using System.Collections;
 using System.Collections.Generic;
using System.Linq;
using InventoryPack;
using ItemPack;
using ItemPack.SO;
using PlayerPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerItemManager : Box
    {
        [SerializeField] private int inventorySize = 8;
        [SerializeField] private int topOpen = 8;
        [SerializeField] private int topClosed = 8;
        [SerializeField] private RectTransform background;
        
        private readonly List<ItemLogicBase> _currentItems = new();
        private List<SoItem> _allItems = new();
        private List<(float weight, SoItem item)> _normalizedWeightItemList = new();
        private float weightSum = 0;

        public delegate void ItemAddDelegate(ItemLogicBase itemLogicBase);
        public static event ItemAddDelegate OnItemAdd;

        private static int CAPACITY => PlayerManager.Instance.PlayerStatsManager.GetStatAsInt(EPlayerStatType.Capacity);
        private GameObject EQ_GRID => gridDataList[1].Grid.gameObject;

        private bool _canInteract;

        protected override void Init()
        {
            _allItems = Resources.LoadAll<SoItem>("Items").Select(Instantiate).ToList();

            var weightItemList = _allItems.Select(item => (weight: 1f / item.ItemPrice, item: item)).ToList();
            weightSum = weightItemList.Sum(w => w.weight);
            _normalizedWeightItemList = weightItemList.Select(pair => (weight: pair.weight / weightSum, item: pair.item)).ToList();
            
            background.offsetMax = new Vector2(background.offsetMax.x, -topClosed);
            EQ_GRID.SetActive(false);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);

            foreach (var slot in _itemSlots)
            {
                var index = slot.Index;
                slot.SetEnabled(index == 0 || (index >= inventorySize && index < CAPACITY + inventorySize));
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.I)) return;

            ToggleEq(!_canInteract);
        }

        public void ToggleEq(bool open)
        {
            _canInteract = open;
            EQ_GRID.SetActive(_canInteract);
            background.offsetMax = new Vector2(background.offsetMax.x, _canInteract ? -topOpen : -topClosed);
        }

        public override bool CanInteract()
        {
            return _canInteract;
        }

        private void AddItemLogic(SoItem itemToAdd, int level)
        {
            var itemLogic = Instantiate(itemToAdd.ItemPrefab);
            itemLogic.transform.localPosition = Vector3.zero;
            itemLogic.Setup(itemToAdd, level);
            _currentItems.Add(itemLogic);

            OnItemAdd?.Invoke(itemLogic);
        }

        public override int AddItem(SoItem item, int level)
        {
            var index = base.AddItem(item, level);
            if (index == -1) return -1;
            
            if (index < 7) AddItemLogic(item, level);
            return index;
        }

        public void RefreshInventory()
        {
            DestroyAllItems();
            foreach (var slot in _itemSlots)
            {
                if (slot.Index >= 7) return; 
                var itemPair = slot.ViewItem();
                if (itemPair.item == null) continue;
                AddItemLogic(itemPair.item, itemPair.level);
            }
        }

        public override void AddItemAtSlot(int index, SoItem item, int level)
        {
            base.AddItemAtSlot(index, item, level);
            RefreshInventory();
        }

        public override void RemoveItemAtSlot(int index)
        {
            base.RemoveItemAtSlot(index);
            RefreshInventory();
        }

        public IEnumerable<SoItem> GetRandomItems(int count, float percentage = 0.0f)
        {
            var selectedItems = new List<SoItem>();

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