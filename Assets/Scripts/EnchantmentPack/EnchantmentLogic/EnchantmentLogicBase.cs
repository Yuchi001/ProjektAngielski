using EnchantmentPack.EnchantmentUIStrategy;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack.EnchantmentLogic
{
    public abstract class EnchantmentLogicBase : MonoBehaviour
    {
        protected SoEnchantment _enchantmentData;
        private int _level;

        public SoEnchantment GetData => _enchantmentData;

        public virtual void AttachDisplayStrategy(EnchantmentItemAccessor accessor) =>
            accessor.gameObject.AddComponent<DefaultDisplayStrategy>().SetLogic(this, accessor);
        
        public virtual void ApplyEnchantment(SoEnchantment data)
        {
            _enchantmentData = data;
        }

        public virtual void RemoveEnchantment()
        {
            Destroy(gameObject);
        }
    }
}