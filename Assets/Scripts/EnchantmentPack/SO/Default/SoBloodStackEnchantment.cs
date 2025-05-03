using EnemyPack;
using UnityEngine;

namespace EnchantmentPack.SO.Default
{
    [CreateAssetMenu(fileName = "new BloodStackEnchantment", menuName = "Custom/Enchantments/BloodStack")]
    public class SoBloodStackEnchantment : SoStackEnchantment
    {
        [SerializeField] private int deathsPerDamage;
        
        public override string GetDescription()
        {
            return Description.Replace("$x$", deathsPerDamage.ToString());
        }

        public override void OnApply(EnchantmentLogic enchantmentLogic)
        {
            enchantmentLogic.SetData(new BloodStackData(this));
            EnemyManager.OnEnemyDeath += enchantmentLogic.GetData<BloodStackData>().RecalculateStacks;
        }
        
        public override void OnRemove(EnchantmentLogic enchantmentLogic)
        {
            EnemyManager.OnEnemyDeath -= enchantmentLogic.GetData<BloodStackData>().RecalculateStacks;
        }

        private class BloodStackData : StackData // wrapper class
        {
            private int _currentDeaths = 0;
            private readonly SoBloodStackEnchantment _data;

            public BloodStackData(SoBloodStackEnchantment data)
            {
                _data = data;
                _currentDeaths = 0;
            }
            public void RecalculateStacks(EnemyLogic enemyLogic)
            {
                _currentDeaths++;
                SetStackCount(_currentDeaths / _data.deathsPerDamage);
            }
        }
    }
}