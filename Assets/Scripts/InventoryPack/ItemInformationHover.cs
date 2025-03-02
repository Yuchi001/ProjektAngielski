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
        
        public void Setup(SoItem item, int level, Box box)
        {
            _box = box;
            _cameraMain = Camera.main;
            itemImage.sprite = item.ItemSprite;
            nameField.text = item.ItemName;
            levelField.text = LevelToStr(level);
            descriptionField.text = item.ItemDescription;
            categoriesField.text = CategoriesToString(item.ItemTags);
            transform.SetPositionToMousePos(_cameraMain);
            
            gameObject.SetActive(true);
        }

        private static string LevelToStr(int level)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < level; i++)
            {
                builder.Append("I");
            }
            return builder.ToString();
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