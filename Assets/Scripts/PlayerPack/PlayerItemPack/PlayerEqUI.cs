using InventoryPack;
using InventoryPack.WorldItemPack;
using TMPro;
using UnityEngine;

namespace PlayerPack.PlayerItemPack
{
    public class PlayerEqUI : Box
    {
        [SerializeField] private TextMeshProUGUI costField;
        [SerializeField] private Color costFieldDefaultColor;
        [SerializeField] private Color costFieldErrorColor;
        
        private const int INGREDIENT_SLOT_1 = 0;
        private const int INGREDIENT_SLOT_2 = 1;
        private const int RESULT_SLOT = 2;

        private int _currentCost = -1;

        protected override void Awake()
        {
            InitBox();
            costField.gameObject.SetActive(false);
            PlayerCollectibleManager.OnCollectibleModify += HandleScrapCountChange;
        }

        protected override Vector2 GetItemDropPosition()
        {
            return PlayerManager.PlayerPos;
        }

        public override void OnOpen(string key)
        {
            base.OnOpen(key);
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
                if (!added) WorldItemManager.SpawnInventoryItem(item.item, PlayerManager.PlayerPos, item.level);
                slot.SetItem(null, -1);
            }
            
        }

        protected override void OnDeactivate()
        {
            Time.timeScale = 1;
            base.OnDeactivate();
        }

        public override void OnSlotChanged(ItemSlot itemSlot)
        {
            if (itemSlot.Index == RESULT_SLOT) HandleResultSlotChange();
            else HandleIngredientSlotChange();
        }

        private void HandleScrapCountChange(PlayerCollectibleManager.ECollectibleType type, int current)
        {
            if (type != PlayerCollectibleManager.ECollectibleType.SCRAP) return;
            
            HandleIngredientSlotChange();
        }

        private void HandleResultSlotChange()
        {
            if (!_itemSlots[RESULT_SLOT].IsEmpty()) return;
            
            if (_currentCost != -1) PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SCRAP, -_currentCost);
            
            foreach (var slot in _itemSlots.GetRange(0, RESULT_SLOT)) slot.SetItem(null, -1);
            costField.gameObject.SetActive(false);

            _currentCost = -1;
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
                _currentCost = -1;
                return;
            }

            if (slot1Empty == slot2Empty) // both slots are not empty
            {
                var sameItemsInBothSlots = item1.item.Is(item2.item);
                if (!sameItemsInBothSlots)
                {
                    _itemSlots[RESULT_SLOT].SetItem(null, -1, false);
                    costField.gameObject.SetActive(false);
                    _currentCost = -1;
                    return;
                }

                var maxLevel = Mathf.Max(item1.level, item2.level) + 1;
                _itemSlots[RESULT_SLOT].SetItem(item1.item, maxLevel, false);
                costField.gameObject.SetActive(false);
                _currentCost = -1;
                return;
            }

            var (item, level) = slot1Empty ? item2 : item1;
            var cost = GetCost(level + 1);
            var canPay = PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.SCRAP) >= cost;
            costField.text = $"Cost <sprite name=\"scraps\">: x{cost}";
            costField.color = canPay ? costFieldDefaultColor : costFieldErrorColor;
            costField.gameObject.SetActive(true);

            if (!canPay)
            {
                _itemSlots[RESULT_SLOT].SetItem(null, -1, false);
                return;
            }
            _itemSlots[RESULT_SLOT].SetItem(item, level + 1, false);
            _currentCost = cost;
        }
        
        private static int GetCost(int level)
        {
            const float a = 1.3f;
            const float b = 2.2f;
            return Mathf.RoundToInt(a * Mathf.Pow(level - 1, b));
        }

        public override bool CanAdd(Box fromBox, ItemSlot itemSlot, int targetSlotIndex)
        {
            if ((itemSlot.Index == RESULT_SLOT && fromBox == this) || targetSlotIndex == RESULT_SLOT) return false;
            return base.CanAdd(fromBox, itemSlot, targetSlotIndex);
        }
    }
}