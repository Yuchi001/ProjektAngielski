using AccessorPack;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using EnemyPack;
using Managers;

namespace EnchantmentPack.Enchantments
{
    public class BleedStacking : EnchantmentBase, IStackEnchantment
    {
        private static EnemySpawner EnemySpawner => MainSceneAccessor.EnemySpawner;
        private int stacksBeforeEnchantment = 0;
        
        private void Start()
        {
            stacksBeforeEnchantment = EnemySpawner.DeadEnemies;
        }

        public int Stacks => (EnemySpawner.DeadEnemies - stacksBeforeEnchantment) / (int)parameters[EValueKey.Value];
    }
}