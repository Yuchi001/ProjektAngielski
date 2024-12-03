using System;
using EnchantmentPack.Interfaces;
using ItemPack;
using ItemPack.SO;
using PlayerPack;

namespace EnchantmentPack.Enchantments
{
    public class BetterBooks : EnchantmentBase, IStackEnchantment
    {
        public int Stacks { get; private set; }

        private void Awake()
        {
            PlayerItemManager.OnItemAdd += OnAdd;
        }

        private void OnUpgrade(SoItem item)
        {
            Stacks++;
        }
        
        private void OnAdd(ItemLogicBase item)
        {
            Stacks++;
        }

        private void OnDisable()
        {
            PlayerItemManager.OnItemAdd -= OnAdd;
        }
    }
}