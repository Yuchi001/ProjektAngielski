using ItemPack.SO;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace InventoryPack
{
    public class DragItemSlot : MonoBehaviour
    {
        private Image _itemImage;
        private ItemSlot _itemSlot;
        private Box _box;
        private Camera _cameraMain;

        private SoInventoryItem InventoryItem => _itemSlot.ViewItem().item;
        private int Level => _itemSlot.ViewItem().level;
        private int Index => _itemSlot.Index;
        
        public void Setup(ItemSlot itemSlot, Box box)
        {
            gameObject.SetActive(false);
            
            _itemSlot = itemSlot;
            _itemImage = GetComponent<Image>();
            _box = box;
            _cameraMain = Camera.main;
            
            transform.position = _itemSlot.transform.position;
        }

        public void BeginDrag()
        {
            _itemImage.sprite = _itemSlot.ViewItem().item.ItemSprite;
            gameObject.SetActive(true);

            transform.SetParent(_box.CurrentCanvas.transform);
            _box.transform.SetSiblingIndex(0);
        }

        public void EndDrag(int targetIndex, Box targetBox)
        {
            EndDrag();

            if (targetBox == _box)
            {
                if (!_box.GetSlotAtIndex(targetIndex).IsEmpty())
                {
                    _box.SwitchItems(Index, targetIndex);
                    return;
                }
                
                _box.AddItemAtSlot(targetIndex, InventoryItem, Level);
                _box.RemoveItemAtSlot(Index);
                return;
            }

            if (targetBox.GetSlotAtIndex(targetIndex).IsEmpty())
            {
                if (!targetBox.CanAdd(InventoryItem)) return;
                
                targetBox.AddItemAtSlot(targetIndex, InventoryItem, Level);
                _box.RemoveItemAtSlot(Index);
                return;
            }

            var item1 = Instantiate(InventoryItem);
            var item1Level = Level;
            var item2 = targetBox.GetSlotAtIndex(targetIndex).ViewItem();

            if (!_box.CanSwitch(item1, item2.item) 
                || !targetBox.CanSwitch(item2.item, item1)) return;
            
            _box.AddItemAtSlot(Index, item2.item, item2.level);
            targetBox.AddItemAtSlot(targetIndex, item1, item1Level);
        }
        
        public void EndDrag()
        {
            var itemSlotTransform = _itemSlot.transform;
            var t = transform;
            t.SetParent(itemSlotTransform);
            
            t.position = itemSlotTransform.position;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.SetPositionToMousePos(_cameraMain);
        }
    }
}