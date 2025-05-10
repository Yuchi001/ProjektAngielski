using System.Collections.Generic;
using System.Linq;
using AudioPack;
using InventoryPack;
using ItemPack;
using ItemPack.SO;
using Managers;
using Managers.Enums;
using Managers.Other;
using PlayerPack.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack.PlayerItemPack
{
    public class PlayerItemManager : Box
    {
        private readonly List<ItemLogicBase> _currentItems = new();
        private List<SoInventoryItem> _allItems = new();
        private List<(float weight, SoInventoryItem item)> _normalizedWeightItemList = new();
        private float weightSum = 0;

        public delegate void ItemAddDelegate(ItemLogicBase itemLogicBase);
        public static event ItemAddDelegate OnItemAdd;

        private static int CAPACITY => PlayerManager.PlayerStatsManager.GetStatAsInt(EPlayerStatType.Capacity);

        private bool _canInteract;

        protected override void Awake()
        {
            var slotPrefab = GameManager.GetPrefab<InventorySlot>(PrefabNames.InventorySlot);
            InitBox(slotPrefab);
            
            _allItems = Resources.LoadAll<SoInventoryItem>("InventoryItems").Select(Instantiate).ToList();
            _allItems.RemoveAll(i => i.ItemName is "Sword" or "Book of Destruction"); // TODO: USUN TO

            var weightItemList = _allItems.Select(item => (weight: 1f / item.ItemPrice, item: item)).ToList();
            weightSum = weightItemList.Sum(w => w.weight);
            _normalizedWeightItemList = weightItemList.Select(pair => (weight: pair.weight / weightSum, item: pair.item)).ToList();

            _canInteract = true;
        }

        protected override Vector2 GetItemDropPosition()
        {
            return PlayerManager.PlayerPos;
        }

        public override void DropItem(int index)
        {
            base.DropItem(index);
            RefreshInventory();
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

        public void ScrapItem(int index)
        {
            var slot = _itemSlots[index];
            var level = slot.ViewItem().level;
            var scrapCount = Mathf.CeilToInt((float)level * level / 2);
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SCRAP, scrapCount);
            slot.SetItem(null, -1);
            AudioManager.PlaySound(ESoundType.ScrapWeapon);
            RefreshInventory();
        }

        public override int AddItem(SoInventoryItem inventoryItem, int level)
        {
            var index = base.AddItem(inventoryItem, level);
            if (index == -1) return -1;
            
            RefreshInventory();
            return index;
        }

        public void CleanInventory()
        {
            DestroyAllItems();
            foreach (var slot in _itemSlots)
            {
                slot.SetItem(null, -1);
                var invSlot = (InventorySlot)slot;
                invSlot.SetSlotState(slot.Index < CAPACITY ? InventorySlot.EState.ACTIVE : InventorySlot.EState.PASSIVE);
            }
        }

        public void RefreshInventory()
        {
            var validSpawnedLogic = new List<int>();
            var missingItemSlots = new List<ItemSlot>();
            
            foreach (var slot in _itemSlots)
            {
                if (slot.Index >= CAPACITY || slot.IsEmpty()) continue;
                var itemPair = slot.ViewItem();

                var spawnedLogic = _currentItems.FirstOrDefault(i =>
                    i.InventoryItem.ItemName == itemPair.item.ItemName && i.Level == itemPair.level);
                if (spawnedLogic != null) validSpawnedLogic.Add(spawnedLogic.GetInstanceID());
                else missingItemSlots.Add(slot);
            }

            _currentItems.RemoveAll(i => i == null);
            foreach (var itemLogic in _currentItems)
            {
                if (validSpawnedLogic.Contains(itemLogic.GetInstanceID())) continue;
                itemLogic.Remove();
            }
            _currentItems.RemoveAll(i => i == null);

            foreach (var slot in missingItemSlots)
            {
                var itemPair = slot.ViewItem();
                AddItemLogic(itemPair.item, itemPair.level);
            }
        }

        public override void SwitchItems(int current, int target)
        {
            var item1 = _itemSlots[current].ViewItem();
            var item2 = _itemSlots[target].ViewItem();

            if (current <= CAPACITY || target <= CAPACITY)
            {
                AddItemAtSlot(target, item1.item, item1.level);
                AddItemAtSlot(current, item2.item, item2.level);
                return;
            }

            base.AddItemAtSlot(target, item1.item, item1.level);
            base.AddItemAtSlot(current, item2.item, item2.level);
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

            /*var item = _allItems[0];
            selectedItems.Clear();
            selectedItems.Add(item);
            selectedItems.Add(item);
            selectedItems.Add(item);
            selectedItems.Add(item);*/
            return selectedItems;
        }

        public void DestroyAllItems()
        {
            foreach (var item in _currentItems.Where(i => i != null)) item.Remove();
            _currentItems.Clear();
        }
    }
}