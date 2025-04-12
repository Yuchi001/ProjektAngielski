using AudioPack;
using Managers.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new SoulItem", menuName = "Custom/Item/SoulItem")]
    public class SoSoulItem : SoItem
    {
        public override bool OnPickUp(params int[] paramArray)
        {
            AudioManager.PlaySound(ESoundType.PickUpGem);
            PlayerManager.PlayerSoulManager.AddSouls(1);
            return true;
        }
    }
}