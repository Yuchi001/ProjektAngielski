using UnityEngine;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public abstract class EnchantmentDisplayStrategy : MonoBehaviour
    {
        public virtual void SetDisplayData(EnchantmentLogic logicBase, EnchantmentItemAccessor uiAccessor)
        {
            uiAccessor.MainImage.gameObject.SetActive(true);
            
            uiAccessor.MainImage.sprite = logicBase.Enchantment.Sprite;
        }
    }
}