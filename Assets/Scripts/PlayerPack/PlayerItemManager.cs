 using System;
 using System.Collections.Generic;
using System.Linq;
using InventoryPack;
using ItemPack;
using ItemPack.SO;
using ItemPack.WeaponPack.WeaponsLogic;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerItemManager : Box
    {
        private readonly List<ItemLogicBase> _currentItems = new();
        private List<SoItem> _allItems = new();
        private List<(float weight, SoItem item)> _normalizedWeightItemList = new();
        private float weightSum = 0;

        public delegate void ItemAddDelegate(ItemLogicBase itemLogicBase);
        public static event ItemAddDelegate OnItemAdd;

        protected override void Init()
        {
            _allItems = Resources.LoadAll<SoItem>("Items").Select(Instantiate).ToList();

            var weightItemList = _allItems.Select(item => (weight: 1f / item.ItemPrice, item: item)).ToList();
            weightSum = weightItemList.Sum(w => w.weight);
            _normalizedWeightItemList = weightItemList.Select(pair => (weight: pair.weight / weightSum, item: pair.item)).ToList();
        }

        private void AddItemLogic(SoItem itemToAdd, int level)
        {
            var itemLogic = Instantiate(itemToAdd.ItemPrefab, transform, true);
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
                AddItemLogic(itemPair.item, itemPair.level);
            }
        }

        public override void AddItemAtSlot(int index, SoItem item, int level)
        {
            base.AddItemAtSlot(index, item, level);
            //RefreshInventory();
        }

        public override void RemoveItemAtSlot(int index)
        {
            base.RemoveItemAtSlot(index);
            //RefreshInventory();
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
        }
    }
}