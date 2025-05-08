using InventoryPack.WorldItemPack;
using ItemPack.Enums;
using ItemPack.SO;
using PlayerPack;
using PlayerPack.PlayerItemPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UIPack.Elements.Button;

namespace ShopPack
{
    public class OfferWindow : MonoBehaviour, IShopUIElement
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI paramText;
        [SerializeField] private GameObject soldObject;
        [SerializeField] private Button offerButton;

        private ShopUI _shopUI;
        private ShopManager.ShopOffer _offer;
        private int _index;
        
        public void SetShop(ShopUI shopUI)
        {
            _shopUI = shopUI;
        }
        
        public void SetOffer(ShopManager.ShopOffer offer, int index)
        {
            soldObject.SetActive(false);
            _offer = offer;
            _index = index;

            itemImage.sprite = _offer.Item.ItemSprite;
            priceText.text = $"Price: {_offer.Price}$";
            paramText.text = $"{(offer.Item.ItemType == EItemType.InventoryItem ? "Level: " : "Count: ")}{_offer.Param}";
            
            priceText.gameObject.SetActive(true);
            paramText.gameObject.SetActive(true);
            itemImage.gameObject.SetActive(true);

            OnUIUpdate();
        }

        public void OnBuy()
        {
            if (_offer.Item is SoInventoryItem inventoryItem && PlayerManager.PlayerItemManager.IsFull())
            {
                if (_shopUI.IsFull()) WorldItemManager.SpawnInventoryItem(inventoryItem, PlayerManager.PlayerPos, _offer.Param);
                else _shopUI.AddItem(inventoryItem, _offer.Param);
            }
            else _offer.Item.OnPickUp(_offer.Param);
            
            ShopManager.BuyItem(_index);
            _offer = null;
            _index = -1;
            itemImage.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            paramText.gameObject.SetActive(false);
            offerButton.EnableButton(false);
            soldObject.SetActive(true);

            _shopUI.UpdateUI();
        }

        public void OnUIUpdate()
        {
            if (_offer == null) return;
            
            var canAfford = ShopManager.CanAfford(_index);
            offerButton.EnableButton(canAfford);
        }
    }
}