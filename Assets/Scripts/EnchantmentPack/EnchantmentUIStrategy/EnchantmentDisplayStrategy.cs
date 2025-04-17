using EnchantmentPack.EnchantmentLogic;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack.EnchantmentUIStrategy
{
    public abstract class EnchantmentDisplayStrategy : MonoBehaviour
    {
        public virtual void SetLogic(EnchantmentLogicBase logicBase, EnchantmentItemAccessor uiAccessor)
        {
            uiAccessor.MainImage.gameObject.SetActive(true);
            
            uiAccessor.MainImage.sprite = logicBase.GetData.Sprite;
        }
    }
}