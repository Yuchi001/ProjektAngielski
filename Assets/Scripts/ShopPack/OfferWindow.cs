using DamageIndicatorPack;
using ItemPack.Enums;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UIPack.Elements.Button;

namespace ShopPack
{
    public class OfferWindow : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI paramText;
        [SerializeField] private GameObject soldObject;
        [SerializeField] private Button offerButton;

        private ShopManager.ShopOffer _offer;
        private int _index;
        

        public void Setup(ShopManager.ShopOffer offer, int index)
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
            offerButton.enabled = true;
        }

        public void OnBuy()
        {
            var success = ShopManager.BuyItem(_index);
            if (success)
            {
                _offer = null;
                itemImage.gameObject.SetActive(false);
                priceText.gameObject.SetActive(false);
                paramText.gameObject.SetActive(false);
                offerButton.enabled = false;
                soldObject.SetActive(true);
                return;
            }
            
            IndicatorManager.SpawnIndicator(itemImage.transform.position, "Cannot buy item!", Color.red);
        }
    }
}