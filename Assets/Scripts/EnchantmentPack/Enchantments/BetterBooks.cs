using System;
using EnchantmentPack.Interfaces;
using PlayerPack;
using WeaponPack;
using WeaponPack.SO;

namespace EnchantmentPack.Enchantments
{
    public class BetterBooks : EnchantmentBase, IStackEnchantment
    {
        public int Stacks { get; private set; }

        private void Awake()
        {
            PlayerWeaponry.OnWeaponLevelUp += OnUpgrade;
            PlayerWeaponry.OnWeaponAdd += OnAdd;
        }

        private void OnUpgrade(SoWeapon weapon)
        {
            Stacks++;
        }
        
        private void OnAdd(WeaponLogicBase weapon)
        {
            Stacks++;
        }
    }
}