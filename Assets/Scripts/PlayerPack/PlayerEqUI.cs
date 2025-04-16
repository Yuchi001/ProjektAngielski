using System.Collections.Generic;
using InventoryPack;
using InventoryPack.WorldItemPack;
using ItemPack.SO;
using TMPro;
using UIPack;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerEqUI : Box
    {
        [SerializeField] private TextMeshProUGUI costField;
        [SerializeField] private Color costFieldDefaultColor;
        [SerializeField] private Color costFieldErrorColor;
        
        private PlayerEqManager _playerEqManager;
        
        private const int INGREDIENT_SLOT_1 = 0;
        private const int INGREDIENT_SLOT_2 = 1;
        private const int RESULT_SLOT = 2;

        public void SetManager(PlayerEqManager playerEqManager)
        {
            _playerEqManager = playerEqManager;
        }
        

        protected override void Awake()
        {
            InitBox();
            costField.gameObject.SetActive(false);
        }

        protected override Vector2 GetItemDropPosition()
        {
            return PlayerManager.PlayerPos;
        }

        protected override void OnOpenAfterAnim()
        {
            Time.timeScale = 0;
        }

        public override void OnClose()
        {
            base.OnClose();
            foreach (var slot in _itemSlots)
            {
                if (slot.IsEmpty() || slot.Index == RESULT_SLOT) continue;

                var item = slot.ViewItem();
                var added= PlayerManager.PlayerItemManager.AddItem(item.item, item.level) != -1;
                if (added) continue;
                
                WorldItemManager.SpawnInventoryItem(item.item, PlayerManager.PlayerPos, item.level);
                slot.SetItem(null, -1);
            }

            Time.timeScale = 1;
        }

        public override void OnSlotChanged(ItemSlot itemSlot)
        {
            if (itemSlot.Index == RESULT_SLOT) HandleResultSlotChange();
            else HandleIngredientSlotChange();
        }

        private void HandleResultSlotChange()
        {
            if (!_itemSlots[RESULT_SLOT].IsEmpty()) return;
            
            foreach (var slot in _itemSlots.GetRange(0, RESULT_SLOT)) slot.SetItem(null, -1);
            //costField.gameObject.SetActive(false);
        }

        private void HandleIngredientSlotChange()
        {
            var slot1Empty = _itemSlots[INGREDIENT_SLOT_1].IsEmpty();
            var slot2Empty = _itemSlots[INGREDIENT_SLOT_2].IsEmpty();

            var item1 = _itemSlots[INGREDIENT_SLOT_1].ViewItem();
            var item2 = _itemSlots[INGREDIENT_SLOT_2].ViewItem();

            if (slot1Empty && slot2Empty)
            {
                _itemSlots[RESULT_SLOT].SetItem(null, -1, false);
                costField.gameObject.SetActive(false);
                return;
            }

            if (slot1Empty == slot2Empty) // both slots are not empty
            {
                var sameItemsInBothSlots = item1.item.Is(item2.item);
                if (!sameItemsInBothSlots)
                {
                    _itemSlots[RESULT_SLOT].SetItem(null, -1, false);
                    costField.gameObject.SetActive(false);
                    return;
                }

                var combinedLevel = item1.level + item2.level;
                _itemSlots[RESULT_SLOT].SetItem(item1.item, combinedLevel, false);
                costField.gameObject.SetActive(true);
                return;
            }

            var (item, cost) = slot1Empty ? item2 : item1;
            var canPay = PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.SCRAP) >= cost;
            costField.text = $"Cost <sprite name=\"scraps\">: x{cost}";
            costField.color = canPay ? costFieldDefaultColor : costFieldErrorColor;
            costField.gameObject.SetActive(true);

            if (!canPay) return;
            _itemSlots[RESULT_SLOT].SetItem(item, cost + 1, false);
        }

        public override bool CanAdd(Box fromBox, ItemSlot itemSlot, int targetSlotIndex)
        {
            if ((itemSlot.Index == RESULT_SLOT && fromBox == this) || targetSlotIndex == RESULT_SLOT) return false;
            return base.CanAdd(fromBox, itemSlot, targetSlotIndex);
        }
    }
}