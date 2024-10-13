using System;
using EnchantmentPack.Interfaces;
using EnemyPack;
using Managers;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class BleedStacking : EnchantmentBase, IStackEnchantment
    {
        [SerializeField] private int stackCount;

        private static EnemySpawner EnemySpawner => GameManager.Instance.WaveManager.EnemySpawner;
        private int stacksBeforeEnchantment = 0;
        
        private void Start()
        {
            stacksBeforeEnchantment = EnemySpawner.DeadEnemies;
        }

        public override string GetDescriptionText()
        {
            return
                $"Gain 1 stack for each {stackCount} kills. Gain 'x' bleed damage, where 'x' is equal to the number of stacks.";
        }

        public int Stacks => (EnemySpawner.DeadEnemies - stacksBeforeEnchantment) / stackCount;
    }
}