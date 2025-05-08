using AudioPack;
using Managers.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new ScrapItem", menuName = "Custom/Item/ScrapItem")]
    public class SoScrapItem : SoItem
    {
        public override bool OnPickUp(params int[] paramArray)
        {
            AudioManager.PlaySound(ESoundType.ScrapWeapon);
            var count = paramArray.Length == 1 ? paramArray[0] : 1;
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SCRAP, count);
            return true;
        }
    }
}