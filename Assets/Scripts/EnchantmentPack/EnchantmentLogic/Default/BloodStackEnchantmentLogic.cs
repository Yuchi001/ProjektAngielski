using System;
using EnchantmentPack.SO.Default;
using EnemyPack;
using Other.Enums;
using PlayerPack;
using PlayerPack.Decorators;
using PlayerPack.Interface;

namespace EnchantmentPack.EnchantmentLogic.Default
{
    public class BloodStackEnchantmentLogic : StackEnchantmentLogic
    {
        private int bloodStacks = 0;
        private SoBloodStackEnchantment _bloodStackData;
        private const string MODIFIER_KEY = "BLOOD_STACK_ENCHANTMENT";
        
        private void Awake()
        {
            EnemySpawner.OnEnemyDie += IncrementStacks;
            var bloodEffectModifier = new BloodEffectModifier(this);
            PlayerManager.GetEffectContextManager().AddEffectModifier(MODIFIER_KEY, bloodEffectModifier);
        }

        private void OnDestroy()
        {
            EnemySpawner.OnEnemyDie -= IncrementStacks;
        }

        private void IncrementStacks(EnemyLogic enemyLogic)
        {
            bloodStacks++;
            _onStacksChange.Invoke(bloodStacks / _bloodStackData.DeathsPerDamage);
        }

        public override void RemoveEnchantment()
        {
            PlayerManager.GetEffectContextManager().RemoveEffectModifier(MODIFIER_KEY);
        }

        private class BloodEffectModifier : IEffectModifier
        {
            private readonly BloodStackEnchantmentLogic _manager;
            
            public BloodEffectModifier(BloodStackEnchantmentLogic manager)
            {
                _manager = manager;
            }
            
            public void ModifyEffectContext(EffectContext effectContext)
            {
                if (effectContext.EffectType != EEffectType.Bleed) return;
                
                effectContext.ModifyAdditionalDamage(_manager.bloodStacks / _manager._bloodStackData.DeathsPerDamage);
            }
        }
    }
}