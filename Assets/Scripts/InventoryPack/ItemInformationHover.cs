using System;
using System.Collections.Generic;
using System.Text;
using ItemPack.Enums;
using ItemPack.SO;
using TMPro;
using UIPack;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace InventoryPack
{
    public class ItemInformationHover : UIBase
    {
        [SerializeField] private TextMeshProUGUI levelField;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private TextMeshProUGUI categoriesField;
        [SerializeField] private Image itemImage;
            
        private Camera _cameraMain;
        private Box _box;
        
        public void Setup(SoInventoryItem inventoryItem, int level, Box box)
        {
            _box = box;
            _cameraMain = Camera.main;
            itemImage.sprite = inventoryItem.ItemSprite;
            nameField.text = inventoryItem.ItemName;
            levelField.text = level.ToString();
            descriptionField.text = inventoryItem.ItemDescription;
            categoriesField.text = CategoriesToString(inventoryItem.ItemTags);
            transform.SetPositionToMousePos(_cameraMain);
            
            gameObject.SetActive(true);
        }

        private static string CategoriesToString(List<EItemTag> tags)
        {
            var builder = new StringBuilder("Categories:\n");
            foreach (var tag in tags)
            {
                builder.Append(tag + "\n");
            }

            return builder.ToString();
        }

        private void Update()
        {
            transform.SetPositionToMousePos(_cameraMain);
            
            if (_box == null || !_box.CanInteract()) Destroy(gameObject);
        }
    }
}