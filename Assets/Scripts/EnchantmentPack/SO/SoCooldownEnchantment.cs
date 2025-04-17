using EnchantmentPack.EnchantmentLogic;
using UnityEngine;

namespace EnchantmentPack.SO
{
    public abstract class SoCooldownEnchantment : SoEnchantment
    {
        [SerializeField] private CooldownEnchantmentLogic logicPrefab;
        [SerializeField] private float cooldown;

        public float Cooldown => cooldown;

        public override EnchantmentLogicBase GetLogicPrefab()
        {
            return logicPrefab;
        }
    }
}