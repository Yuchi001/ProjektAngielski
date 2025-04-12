using AudioPack;
using Managers.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new CoinItem", menuName = "Custom/Item/CoinItem")]
    public class SoCoinItem : SoItem
    {
        public override bool OnPickUp(params int[] paramArray)
        {
            AudioManager.PlaySound(ESoundType.PickUpGem);
            PlayerManager.PlayerCoinManager.AddCoins(paramArray[0]);
            return true;
        }
    }
}