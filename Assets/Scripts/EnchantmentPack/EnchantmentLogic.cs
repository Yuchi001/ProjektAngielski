using System;
using EnchantmentPack.EnchantmentUIStrategy;
using EnchantmentPack.SO;
using UnityEngine;

namespace EnchantmentPack
{
    public class EnchantmentLogic : MonoBehaviour
    {
        public SoEnchantment Enchantment { get; private set; }
        private EnchantmentItemAccessor _accessor;
        private Data _data;

        public void Add(SoEnchantment enchantment)
        {
            Enchantment = enchantment;
            Enchantment.OnApply(this);
        }

        private void Update()
        {
            Enchantment.OnUpdate(this);
        }

        public void Remove()
        {
            Enchantment.OnRemove(this);
            Destroy(_accessor.gameObject);
            Destroy(gameObject);
        }

        public void ApplyDisplayStrategy(EnchantmentItemAccessor accessor)
        {
            _accessor = accessor;
            Enchantment.HandleDisplayStrategy(_accessor, this);
        }
            

        public void SetData(Data data) 
        {
            _data = data;
        }

        public T GetData<T>() where T : Data
        {
            return (T)_data;
        }

        public abstract class Data
        {
        }
    }
}