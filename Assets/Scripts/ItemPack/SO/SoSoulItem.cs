using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new SoulItem", menuName = "Custom/Item/SoulItem")]
    public class SoSoulItem : SoItem
    {
        public override bool OnPickUp(params int[] paramArray)
        {
            //TODO implementacja dusz
            return false;
        }
    }
}