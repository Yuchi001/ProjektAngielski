using PlayerPack;
using Button = UIPack.Elements.Button;

namespace ShopPack
{
    public class RefreshOffersButton : Button, IShopUIElement
    {
        private ShopUI _shopUI;
        
        public void SetShop(ShopUI shopUI)
        {
            _shopUI = shopUI;
            _textField.text = $"R E F R E S H {ShopManager.RefreshCost}$";
            OnUIUpdate();
        }

        public void OnRefresh()
        {
            _shopUI.RefreshOffers();
        }

        public void OnUIUpdate()
        {
            var coins = PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.COIN);
            var canBuy = coins >= ShopManager.RefreshCost;
            
            EnableButton(canBuy);
        }
    }
}